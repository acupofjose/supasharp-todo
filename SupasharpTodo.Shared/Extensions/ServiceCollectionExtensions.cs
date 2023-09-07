using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Supabase;
using SupasharpTodo.Shared.Interfaces;
using SupasharpTodo.Shared.Providers;
using SupasharpTodo.Shared.Services;

namespace SupasharpTodo.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// A helper extension to register the appropriate shared code.
    ///
    /// Requires that an <see cref="ILocalStorageProvider"/> has been registered as a Scoped Blazor service
    /// </summary>
    /// <param name="services"></param>
    public static void AddSupasharpTodoSharedCore(this IServiceCollection services)
    {
        services.AddScoped(provider =>
        {
            var url = "https://ohthynqufdglbdtoplcb.supabase.co";
            var publicKey =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im9odGh5bnF1ZmRnbGJkdG9wbGNiIiwicm9sZSI6ImFub24iLCJpYXQiOjE2OTM1Njk5NDYsImV4cCI6MjAwOTE0NTk0Nn0.YxuyLRaAivc2sUEJQ0JLSv0MoTIvu_-9CnBFU8fhnrI";

            var localStorageProvider = provider.GetRequiredService<ILocalStorageProvider>();
            return new Client(url, publicKey, new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
                SessionHandler = new SupabaseSessionService(localStorageProvider!)
            });
        });

        services.AddScoped<IAppStateService>(p => new AppStateService(p.GetRequiredService<Client>()));
        services.AddScoped<ITodoService>(p =>
            new TodoService(p.GetRequiredService<IAppStateService>(), p.GetRequiredService<Client>()));

        Console.WriteLine("Initialized Supabase Core.");
    }
}