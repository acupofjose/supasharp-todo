using System.ComponentModel;
using System.Runtime.CompilerServices;
using Postgrest.Exceptions;
using Postgrest.Interfaces;
using Supabase.Realtime;
using Supabase.Realtime.Exceptions;
using Supabase.Realtime.Interfaces;
using Supabase.Realtime.PostgresChanges;
using SupasharpTodo.Shared.Extensions;
using SupasharpTodo.Shared.Interfaces;
using SupasharpTodo.Shared.Models;
using SupasharpTodo.Shared.Utilities;

namespace SupasharpTodo.Shared.Services;

/// <summary>
/// The Todo Service handles maintaining a local copy of all Todos for this user and facilitates any interactions that a user would
/// make on a todo.
/// </summary>
public sealed class TodoService : ITodoService
{
    /// <summary>
    /// The stored collection of Todos (raises an event every time the collection, or an individual todo, is changed.
    /// </summary>
    public FullyObservableCollection<Todo> Todos { get; set; } = new();

    private bool _isLoading;

    /// <summary>
    /// If the service is presently loading.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        private set => SetField(ref _isLoading, value);
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    private Supabase.Client Supabase { get; }
    private IAppStateService AppStateService { get; }
    private IPostgrestCacheProvider PostgrestCacheProvider { get; }
    private IPostgrestTableWithCache<Todo> Ref => Supabase.Postgrest.Table<Todo>(PostgrestCacheProvider);
    private RealtimeChannel? Listener { get; set; }

    /// <summary>
    /// Initializes the Todo Service
    /// </summary>
    /// <param name="appStateService">Maintains a global error state</param>
    /// <param name="supabase">Backend persistence</param>
    /// <param name="postgrestCacheProvider">Local Persistence</param>
    public TodoService(IAppStateService appStateService, Supabase.Client supabase,
        IPostgrestCacheProvider postgrestCacheProvider)
    {
        AppStateService = appStateService;
        Supabase = supabase;
        PostgrestCacheProvider = postgrestCacheProvider;

        AppStateService.PropertyChanged += AppStateServiceOnPropertyChanged;

        if (AppStateService.IsLoggedIn)
            Register();
    }

    /// <summary>
    /// Creates a Todo, stores it in memory, and sends it to <see cref="Supabase"/>
    /// </summary>
    /// <param name="todo"></param>
    /// <returns></returns>
    public async Task<bool> Create(Todo todo)
    {
        try
        {
            todo.UserId = AppStateService.User.Id!;
            todo.CreatedAt = DateTime.UtcNow;
            todo.UpdatedAt = DateTime.UtcNow;

            Todos.Add(todo);

            try
            {
                var inserted = await Ref.Insert(todo);
                todo.Id = inserted.Model!.Id;
                return true;
            }
            catch (PostgrestException ex)
            {
                Console.WriteLine(ex.Message);
                Todos.Remove(todo);
                AppStateService.Errors.Add(ex.Message);
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            AppStateService.Errors.Add(ex.Message);
            IsLoading = false;
            return false;
        }
    }

    /// <summary>
    /// Marks a todo as Completed/Incompleted locally and then syncs state with <see cref="Supabase"/>
    /// </summary>
    /// <param name="todo"></param>
    /// <param name="isCompleted"></param>
    /// <returns></returns>
    public async Task<bool> MarkCompletion(Todo todo, bool isCompleted)
    {
        try
        {
            todo.CompletedAt = isCompleted ? DateTime.UtcNow : null;
            await Ref.Update(todo);
            return true;
        }
        catch (Exception ex)
        {
            AppStateService.Errors.Add(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Duplicates a todo locally and then creates a new duplicated todo in <see cref="Supabase"/>
    /// </summary>
    /// <param name="todo"></param>
    /// <returns></returns>
    public async Task<bool> Duplicate(Todo todo)
    {
        try
        {
            var duplicatedTodo = todo.DeepCopy();
            todo.Id = string.Empty;
            todo.UpdatedAt = DateTime.UtcNow;
            await Ref.Insert(duplicatedTodo);
            return true;
        }
        catch (PostgrestException ex)
        {
            AppStateService.Errors.Add(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// "Restores" a todo that was previously trashed.
    /// </summary>
    /// <param name="todo"></param>
    /// <returns></returns>
    public async Task<bool> Restore(Todo todo)
    {
        try
        {
            todo.TrashedAt = null;
            await Ref.Insert(todo);
            return true;
        }
        catch (PostgrestException ex)
        {
            AppStateService.Errors.Add(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Moves a todo to the trash.
    /// </summary>
    /// <param name="todo"></param>
    /// <returns></returns>
    public async Task<bool> MoveToTrash(Todo todo)
    {
        try
        {
            todo.TrashedAt = DateTime.UtcNow;
            await Ref.Insert(todo);
            return true;
        }
        catch (PostgrestException ex)
        {
            AppStateService.Errors.Add(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Updates a todo in <see cref="Supabase"/>
    /// </summary>
    /// <param name="todo"></param>
    /// <returns></returns>
    public async Task<bool> Update(Todo todo)
    {
        try
        {
            await Ref.Update(todo);
            return true;
        }
        catch (PostgrestException ex)
        {
            AppStateService.Errors.Add(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Deletes a todo.
    /// </summary>
    /// <param name="todo"></param>
    /// <returns></returns>
    public async Task<bool> Delete(Todo todo)
    {
        try
        {
            await Ref.Delete(todo);
            Todos.Remove(todo);
            return true;
        }
        catch (Exception ex)
        {
            AppStateService.Errors.Add(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Handles changes to App State, specifically checking when a user signs in so that we can populate the
    /// User's todos and register a realtime listener
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AppStateServiceOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (AppStateService.IsLoggedIn)
            Register();
        else
            Unregister();
    }

    /// <summary>
    /// Registers the realtime listener and <see cref="Populate"/>s the local cache.
    /// </summary>
    private async void Register()
    {
        IsLoading = true;

        await Supabase.InitializeAsync();

        try
        {
            Listener ??= await Supabase.From<Todo>().On(PostgresChangesOptions.ListenType.All, OnTodoModelChanges);
        }
        catch (RealtimeException ex)
        {
            Console.WriteLine(ex.Message);
            AppStateService.Errors.Add(ex.Message);
        }

        await Populate();

        IsLoading = false;
    }
    
    /// <summary>
    /// Handles changes registered from the <see cref="Listener"/>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="change"></param>
    private void OnTodoModelChanges(IRealtimeChannel sender, PostgresChangesResponse change)
    {
        var model = change.Model<Todo>();

        switch (change.Payload?.Data?._type)
        {
            case "INSERT":
                Console.WriteLine($"Todo has been inserted: {model.Id}");
                var existing = Todos.FirstOrDefault(t => t.Id == model.Id);
                if (existing == null)
                    Todos.Add(model);
                break;
            case "UPDATE":
                Console.WriteLine($"Todo has been updated: {model.Id}");
                var toBeUpdated = Todos.FirstOrDefault(t => t.Id == model!.Id);
                Todos[Todos.IndexOf(toBeUpdated)] = model;
                break;
            case "DELETE":
                Console.WriteLine($"Todo has been deleted: {model.Id}");
                var toBeRemoved = Todos.FirstOrDefault(t => t.Id == model!.Id);
                Todos.Remove(toBeRemoved);
                break;
        }
    }

    /// <summary>
    /// Removes the realtime listener and clears <see cref="Todos"/>
    /// </summary>
    private void Unregister()
    {
        Listener?.Unsubscribe();
        Listener = null;
        Todos.Clear();
    }

    /// <summary>
    /// Populates local <see cref="Todos"/> with a hard pull from <see cref="Supabase"/>. Initially populates from
    /// the <see cref="PostgrestCacheProvider"/> if possible.
    /// </summary>
    private async Task Populate()
    {
        try
        {
            var response = await Ref.Get();

            foreach (var model in response.Models)
                Todos.Add(model);

            response.RemoteModelsPopulated += sender =>
            {
                foreach (var model in sender.Models)
                {
                    var existing = Todos.FirstOrDefault(t => t.Id == model.Id);
                    Todos[Todos.IndexOf(existing)] = model;
                }
            };
        }
        catch (PostgrestException ex)
        {
            Console.WriteLine(ex.Message);
            AppStateService.Errors.Add(ex.Message);
        }
    }
    
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}