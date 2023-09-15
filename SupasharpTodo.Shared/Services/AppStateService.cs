using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using SupasharpTodo.Shared.Interfaces;

namespace SupasharpTodo.Shared.Services
{
    public sealed class AppStateService : IAppStateService
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public IAppStateService.AppSection FocusedAppSection { get; private set; } =
            IAppStateService.AppSection.TodoList;

        private List<IAppStateService.FocusedAppSectionChanged> _focusedAppSectionChangedListeners = new();

        public ObservableCollection<string> Errors { get; } = new();

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

        public void AddFocusedAppSectionChangedListener(IAppStateService.FocusedAppSectionChanged listener)
        {
            if (!_focusedAppSectionChangedListeners.Contains(listener))
                _focusedAppSectionChangedListeners.Add(listener);
        }

        public void RemoveFocusedAppSectionChangedListener(IAppStateService.FocusedAppSectionChanged listener)
        {
            if (_focusedAppSectionChangedListeners.Contains(listener))
                _focusedAppSectionChangedListeners.Remove(listener);
        }

        public void ClearFocusedAppSectionChangedListeners(IAppStateService.FocusedAppSectionChanged listener) =>
            _focusedAppSectionChangedListeners.Clear();

        public void SetFocusedAppSection(IAppStateService.AppSection gainedFocus)
        {
            var lostFocus = FocusedAppSection;
            FocusedAppSection = gainedFocus;

            NotifyFocusedAppSectionChanged(lostFocus, gainedFocus);
        }

        private void NotifyFocusedAppSectionChanged(IAppStateService.AppSection lostFocus,
            IAppStateService.AppSection gainedFocus)
        {
            if (lostFocus == gainedFocus) return;

            foreach (var listener in _focusedAppSectionChangedListeners.ToList())
                listener.Invoke(lostFocus, gainedFocus);
        }

        private Supabase.Client Supabase { get; }

        public AppStateService(Supabase.Client client)
        {
            Supabase = client;
            Supabase.Auth.AddStateChangedListener(AuthEventHandler);

            if (Supabase.Auth.CurrentUser != null)
                IsLoggedIn = true;
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