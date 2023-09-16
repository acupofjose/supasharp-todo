using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Supabase;
using SupasharpTodo.MauiBlazor.Services;
using SupasharpTodo.Shared.Extensions;
using SupasharpTodo.Shared.Interfaces;
using SupasharpTodo.Shared.Providers;
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
        builder.Services.AddScoped<ILocalStorageProvider, LocalStorageProvider>();
        builder.Services.AddSupasharpTodoSharedCore();

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