using System.Collections.ObjectModel;
using System.ComponentModel;
using SupasharpTodo.Shared.Models;

namespace SupasharpTodo.Shared.Interfaces;

public interface ITodoService : INotifyPropertyChanged
{
    bool IsLoading { get; }
    ObservableCollection<Todo> Todos { get; set; }
    Task<bool> Create(Todo todo);
    Task<bool> MoveToTrash(Todo todo);
    Task<bool> MarkCompletion(Todo todo, bool isCompleted);
    Task<bool> Delete(Todo todo);
}