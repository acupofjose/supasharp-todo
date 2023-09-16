using SupasharpTodo.Shared.Interfaces;

namespace SupasharpTodo.Shared.Services;

public class SyncService
{
    public Supabase.Client Supabase { get; private set; }
    public ILocalStorageProvider LocalStorageProvider { get; private set; }

    public SyncService(ILocalStorageProvider localStorageProvider, Supabase.Client remoteStorageProvider)
    {
        LocalStorageProvider = localStorageProvider;
        Supabase = remoteStorageProvider;
    }
}