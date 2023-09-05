using System.Collections.ObjectModel;
using System.ComponentModel;
using Supabase.Gotrue;

namespace SupasharpTodo.Shared.Interfaces
{
    public interface IAppStateService : INotifyPropertyChanged
    {
        ObservableCollection<string> Errors { get; }
        bool IsLoading { get; }
        User User { get; }
        bool IsLoggedIn { get; }
        string? AvatarUrl { get; }
    }
}