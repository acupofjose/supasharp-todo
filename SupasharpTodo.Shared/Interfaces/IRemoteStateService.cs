namespace SupasharpTodo.Shared.Interfaces;

public interface IRemoteStateService
{
    Task SaveState();
    Task RestoreState();
}