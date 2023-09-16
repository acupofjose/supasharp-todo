using Microsoft.Extensions.DependencyInjection;
using Postgrest.Interfaces;
using Supabase;
using SupasharpTodo.Shared.Interfaces;
using SupasharpTodo.Shared.Providers;
using SupasharpTodo.Shared.Services;

namespace SupasharpTodo.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// A helper extension to register the shared code and its dependencies.
    ///
    /// Requires that an <see cref="ILocalStorageProvider"/> has been registered as a Scoped Blazor service
    /// </summary>
    /// <param name="services"></param>
    public static void AddSupasharpTodoSharedCore(this IServiceCollection services)
    {
        // Register Supabase with its session provider
        services.AddScoped(provider =>
        {
            const string url = "https://ohthynqufdglbdtoplcb.supabase.co";
            const string publicKey =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im9odGh5bnF1ZmRnbGJkdG9wbGNiIiwicm9sZSI6ImFub24iLCJpYXQiOjE2OTM1Njk5NDYsImV4cCI6MjAwOTE0NTk0Nn0.YxuyLRaAivc2sUEJQ0JLSv0MoTIvu_-9CnBFU8fhnrI";

            var localStorageProvider = provider.GetRequiredService<ILocalStorageProvider>();
            return new Client(url, publicKey, new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
                SessionHandler = new SupabaseSessionProvider(localStorageProvider!)
            });
        });

        // Register a postgrest cache provider
        services.AddScoped<IPostgrestCacheProvider, PostgrestCacheProvider>(p =>
            new PostgrestCacheProvider(p.GetRequiredService<ILocalStorageProvider>()));

        // Register State Handlers and Services
        services.AddScoped<IAppStateService>(p => new AppStateService(p.GetRequiredService<Client>()));
        services.AddScoped<ITodoService>(p =>
            new TodoService(p.GetRequiredService<IAppStateService>(),
                p.GetRequiredService<Client>(),
                p.GetRequiredService<IPostgrestCacheProvider>()));

        Console.WriteLine("Initialized Supabase Core.");
    }
}