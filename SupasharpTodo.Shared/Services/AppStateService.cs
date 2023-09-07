using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using SupasharpTodo.Shared.Interfaces;

namespace SupasharpTodo.Shared.Services
{
    public sealed class AppStateService : IAppStateService
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly List<IAppStateService.CurrentElementRefChangedHandler> _currentElementRefChangedHandlers =
            new();

        public ObservableCollection<string> Errors { get; } = new();

        public ElementReference? CurrentElementRef { get; private set; }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetField(ref _isLoading, value);
        }

        private bool _isLoggedIn;

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set => SetField(ref _isLoggedIn, value);
        }

        public User? User => Supabase.Auth.CurrentUser;

        public string? AvatarUrl
        {
            get
            {
                if (User != null && User.UserMetadata.TryGetValue("avatar_url", out var avatarUrl))
                {
                    return avatarUrl.ToString();
                }

                return null;
            }
        }

        public void SetCurrentElementRef(ElementReference? elementReference)
        {
            if (elementReference.Equals(CurrentElementRef)) return;

            var oldRef = elementReference;
            CurrentElementRef = elementReference;
            NotifyCurrentElementRefChanged(oldRef, CurrentElementRef);
        }

        public void AddCurrentElementRefChangedHandler(IAppStateService.CurrentElementRefChangedHandler handler)
        {
            if (!_currentElementRefChangedHandlers.Contains(handler))
                _currentElementRefChangedHandlers.Add(handler);
        }

        public void RemoveCurrentElementRefChangedHandler(IAppStateService.CurrentElementRefChangedHandler handler)
        {
            if (_currentElementRefChangedHandlers.Contains(handler))
                _currentElementRefChangedHandlers.Remove(handler);
        }

        public void ClearCurrentElementRefChangedHandler() => _currentElementRefChangedHandlers.Clear();

        private void NotifyCurrentElementRefChanged(ElementReference? oldRef, ElementReference? newRef)
        {
            foreach (var handler in _currentElementRefChangedHandlers)
            {
                try
                {
                    handler.Invoke(this, oldRef, newRef);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private Supabase.Client Supabase { get; }

        public AppStateService(Supabase.Client client)
        {
            Supabase = client;
            Supabase.Auth.AddStateChangedListener(AuthEventHandler);

            if (Supabase.Auth.CurrentUser != null)
            {
                IsLoggedIn = true;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            OnPropertyChanged(propertyName);
        }

        private void AuthEventHandler(IGotrueClient<User, Session> sender, Constants.AuthState state)
        {
            IsLoggedIn = state switch
            {
                Constants.AuthState.SignedIn => true,
                Constants.AuthState.SignedOut => false,
                _ => IsLoggedIn
            };
        }
    }
}