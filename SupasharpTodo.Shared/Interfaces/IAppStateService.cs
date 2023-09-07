using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using Supabase.Gotrue;

namespace SupasharpTodo.Shared.Interfaces
{
    public interface IAppStateService : INotifyPropertyChanged
    {
        ElementReference? CurrentElementRef { get; }

        delegate void CurrentElementRefChangedHandler(IAppStateService service, ElementReference? oldRef,
            ElementReference? newRef);

        ObservableCollection<string> Errors { get; }
        bool IsLoading { get; }
        User User { get; }
        bool IsLoggedIn { get; }
        string? AvatarUrl { get; }

        void SetCurrentElementRef(ElementReference? elementReference);
        void AddCurrentElementRefChangedHandler(CurrentElementRefChangedHandler handler);
        void RemoveCurrentElementRefChangedHandler(CurrentElementRefChangedHandler handler);
        void ClearCurrentElementRefChangedHandler();
    }
}