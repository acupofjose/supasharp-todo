using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Postgrest.Exceptions;
using Supabase.Realtime;
using Supabase.Realtime.Exceptions;
using Supabase.Realtime.Interfaces;
using Supabase.Realtime.PostgresChanges;
using SupasharpTodo.Shared.Interfaces;
using SupasharpTodo.Shared.Models;
using SupasharpTodo.Shared.Utilities;

namespace SupasharpTodo.Shared.Services;

public class TodoService : ITodoService
{
    public FullyObservableCollection<Todo> Todos { get; set; } = new();

    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        private set => SetField(ref _isLoading, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private Supabase.Client Supabase { get; }

    private IAppStateService AppStateService { get; }

    private RealtimeChannel? Listener { get; set; }

    public TodoService(IAppStateService appStateService, Supabase.Client supabase)
    {
        AppStateService = appStateService;
        Supabase = supabase;

        AppStateService.PropertyChanged += AppStateServiceOnPropertyChanged;

        if (AppStateService.IsLoggedIn)
            Register();
    }

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
                await Supabase.From<Todo>().Insert(todo);
            }
            catch (PostgrestException ex)
            {
                Console.WriteLine(ex.Message);
                Todos.Remove(todo);
                AppStateService.Errors.Add(ex.Message);
                return false;
            }
            OnPropertyChanged(nameof(Todos));
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            AppStateService.Errors.Add(ex.Message);
            IsLoading = false;
            return false;
        }
    }

    public async Task<bool> MarkCompletion(Todo todo, bool isCompleted)
    {
        try
        {
            todo.CompletedAt = isCompleted ? DateTime.UtcNow : null;
            await todo.Update<Todo>();
            OnPropertyChanged(nameof(Todos));
            return true;
        }
        catch (Exception ex)
        {
            AppStateService.Errors.Add(ex.Message);
            return false;
        }
    }

    public async Task<bool> MoveToTrash(Todo todo)
    {
        try
        {
            todo.TrashedAt = DateTime.UtcNow;
            await todo.Update<Todo>();
            OnPropertyChanged(nameof(Todos));
            return true;
        }
        catch (PostgrestException ex)
        {
            AppStateService.Errors.Add(ex.Message);
            return false;
        }
    }

    public async Task<bool> Update(Todo todo)
    {
        try
        {
            await todo.Update<Todo>();
            OnPropertyChanged(nameof(Todos));
            return true;
        }
        catch (PostgrestException ex)
        {
            AppStateService.Errors.Add(ex.Message);
            return false;
        }
    }

    public async Task<bool> Delete(Todo todo)
    {
        try
        {
            await todo.Delete<Todo>();
            OnPropertyChanged(nameof(Todos));
            return true;
        }
        catch (Exception ex)
        {
            AppStateService.Errors.Add(ex.Message);
            return false;
        }
    }

    private void AppStateServiceOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (AppStateService.IsLoggedIn)
            Register();
        else
            Unregister();
    }

    private async void Register()
    {
        IsLoading = true;

        await Supabase.InitializeAsync();

        try
        {
            Listener = await Supabase.From<Todo>().On(PostgresChangesOptions.ListenType.All, OnTodoModelChanges);
        }
        catch (RealtimeException ex)
        {
            Console.WriteLine(ex.Message);
            AppStateService.Errors.Add(ex.Message);
        }

        try
        {
            var response = await Supabase.From<Todo>().Get();

            foreach (var model in response.Models)
                Todos.Add(model);
        }
        catch (PostgrestException ex)
        {
            Console.WriteLine(ex.Message);
            AppStateService.Errors.Add(ex.Message);
        }

        IsLoading = false;
    }

    private void Unregister()
    {
        if (Listener != null)
        {
            Listener.Unsubscribe();
            Listener = null;
        }

        Todos.Clear();
    }

    private void OnTodoModelChanges(IRealtimeChannel sender, PostgresChangesResponse change)
    {
        var model = change.Model<Todo>();

        switch (change.Event)
        {
            case Constants.EventType.Insert:
                Todos.Add(model);
                break;
            case Constants.EventType.Update:
                var toBeUpdated = Todos.FirstOrDefault(t => t.Id == model!.Id);
                Todos[Todos.IndexOf(toBeUpdated)] = model;
                break;
            case Constants.EventType.Delete:
                var toBeRemoved = Todos.FirstOrDefault(t => t.Id == model!.Id);
                Todos.Remove(toBeRemoved);
                break;
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}