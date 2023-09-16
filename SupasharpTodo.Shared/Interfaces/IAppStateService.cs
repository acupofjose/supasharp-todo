using System.Collections.ObjectModel;
using System.ComponentModel;
using Supabase.Gotrue;

namespace SupasharpTodo.Shared.Interfaces
{
    public interface IAppStateService : INotifyPropertyChanged
    {
        #region General Properties

        ObservableCollection<string> Errors { get; }
        bool IsLoading { get; }
        User User { get; }
        bool IsLoggedIn { get; }
        string? AvatarUrl { get; }

        #endregion


        #region App Section Focus Changed

        enum AppSection
        {
            Menu,
            TodoList
        }

        AppSection FocusedAppSection { get; }

        delegate void FocusedAppSectionChanged(AppSection lostFocus, AppSection gainedFocus);

        void AddFocusedAppSectionChangedListener(FocusedAppSectionChanged listener);
        void RemoveFocusedAppSectionChangedListener(FocusedAppSectionChanged listener);
        void ClearFocusedAppSectionChangedListeners(FocusedAppSectionChanged listener);
        void SetFocusedAppSection(AppSection gainedFocus);

        #endregion
    }
}