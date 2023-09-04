using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using SupasharpTodo.Shared.Interfaces;
using SupasharpTodo.Shared.Models;

namespace SupasharpTodo.Shared.Services
{
    public sealed class AppStateService : IAppStateService
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<string> Errors { get; } = new();

        public User? User => Supabase.Auth.CurrentUser;

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

        public ObservableCollection<Todo> Todos { get; } = new();

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