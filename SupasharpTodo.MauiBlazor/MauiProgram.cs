using Microsoft.Extensions.Logging;
using Supabase;
using SupasharpTodo.MauiBlazor.Services;
using SupasharpTodo.Shared.Interfaces;
using SupasharpTodo.Shared.Services;

namespace SupasharpTodo.MauiBlazor;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        builder.Services.AddMauiBlazorWebView();

        builder.Services.AddBlazorWebViewDeveloperTools();
#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Supabase
        builder.Services.AddSingleton(provider =>
        {
            var url = "https://ohthynqufdglbdtoplcb.supabase.co";
            var publicKey =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im9odGh5bnF1ZmRnbGJkdG9wbGNiIiwicm9sZSI6ImFub24iLCJpYXQiOjE2OTM1Njk5NDYsImV4cCI6MjAwOTE0NTk0Nn0.YxuyLRaAivc2sUEJQ0JLSv0MoTIvu_-9CnBFU8fhnrI";

            var localStorageProvider = new LocalStorageProvider();
            return new Client(url, publicKey, new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
                SessionHandler = new SupabaseSessionService(localStorageProvider)
            });
        });
        
        builder.Services.AddScoped<IAppStateService>(p => new AppStateService(p.GetRequiredService<Client>()));
        builder.Services.AddScoped<ITodoService>(p =>
            new TodoService(p.GetRequiredService<IAppStateService>(), p.GetRequiredService<Client>()));

        var app = builder.Build();

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var supabase = app.Services.GetRequiredService<Client>();
            await supabase.InitializeAsync();
            supabase.Auth.LoadSession();
        });

        return app;
    }
}