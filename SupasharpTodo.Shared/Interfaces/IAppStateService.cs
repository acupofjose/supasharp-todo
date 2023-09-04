using System.Collections.ObjectModel;
using System.ComponentModel;
using Supabase.Gotrue;
using SupasharpTodo.Shared.Models;

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