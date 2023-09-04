using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Supabase;
using SupasharpTodo.BlazorWASM;
using SupasharpTodo.BlazorWASM.Services;
using SupasharpTodo.Shared.Services;
using Blazored.LocalStorage;
using SupasharpTodo.Shared.Interfaces;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Plugins
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddHotKeys2();

// Supabase
builder.Services.AddScoped(provider =>
{
    var url = "https://ohthynqufdglbdtoplcb.supabase.co";
    var publicKey =
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im9odGh5bnF1ZmRnbGJkdG9wbGNiIiwicm9sZSI6ImFub24iLCJpYXQiOjE2OTM1Njk5NDYsImV4cCI6MjAwOTE0NTk0Nn0.YxuyLRaAivc2sUEJQ0JLSv0MoTIvu_-9CnBFU8fhnrI";

    var blazorStorage = provider.GetRequiredService<ISyncLocalStorageService>();
    var localStorageProvider = new LocalStorageProvider(blazorStorage);

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

// Init Supabase
var supabase = app.Services.GetRequiredService<Client>();
await supabase.InitializeAsync();
supabase.Auth.LoadSession();

await app.RunAsync();