using Newtonsoft.Json;
using SupasharpTodo.Shared.Interfaces;

namespace SupasharpTodo.Shared.Providers;

/// <summary>
/// This cache provider handles storing a particular query (identified by its url) into a local caching strategy.
/// </summary>
public class PostgrestCacheProvider : Postgrest.Interfaces.IPostgrestCacheProvider
{
    private readonly ILocalStorageProvider _localStorageProvider;

    public PostgrestCacheProvider(ILocalStorageProvider localStorageProvider)
    {
        _localStorageProvider = localStorageProvider;
    }
    
    public Task<T?> GetItem<T>(string key)
    {
        var stored = _localStorageProvider.GetItem(key);

        if (string.IsNullOrEmpty(stored))
            return Task.FromResult<T?>(default);

        var deserialized = JsonConvert.DeserializeObject<T>(stored);
        
        if (deserialized == null) 
            return Task.FromResult<T?>(default);
        
        return Task.FromResult(deserialized);
    }

    public Task SetItem(string key, object value)
    {
        var serialized = JsonConvert.SerializeObject(value);
        _localStorageProvider.SetItem(key, serialized);
        
        return Task.CompletedTask;
    }

    public Task ClearItem(string key)
    {
        _localStorageProvider.RemoveItem(key);
        return Task.CompletedTask;
    }

    public Task Empty()
    {
        _localStorageProvider.Empty();
        return Task.CompletedTask;
    }
}