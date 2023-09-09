using System.Collections.ObjectModel;
using System.ComponentModel;
using SupasharpTodo.Shared.Models;
using SupasharpTodo.Shared.Utilities;

namespace SupasharpTodo.Shared.Interfaces;

public interface ITodoService : INotifyPropertyChanged
{
    bool IsLoading { get; }
    FullyObservableCollection<Todo> Todos { get; set; }
    Task<bool> Create(Todo todo);
    Task<bool> MoveToTrash(Todo todo);
    Task<bool> Restore(Todo todo);
    Task<bool> Duplicate(Todo todo);
    Task<bool> MarkCompletion(Todo todo, bool isCompleted);
    Task<bool> Delete(Todo todo);
    Task<bool> Update(Todo todo);
}