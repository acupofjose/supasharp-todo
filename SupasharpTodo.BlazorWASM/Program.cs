using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Supabase;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SupasharpTodo.BlazorWASM;
using SupasharpTodo.BlazorWASM.Services;
using SupasharpTodo.Shared.Extensions;
using SupasharpTodo.Shared.Interfaces;
using SupasharpTodo.Shared.Providers;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Plugins
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddHotKeys2();

// Core
builder.Services.AddScoped<ILocalStorageProvider>(p =>
    new LocalStorageProvider(p.GetRequiredService<ISyncLocalStorageService>()));
builder.Services.AddSupasharpTodoSharedCore();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider>(p =>
    new SupabaseAuthenticationStateProvider(p.GetRequiredService<Client>()));

var app = builder.Build();

// Init Supabase
var supabase = app.Services.GetRequiredService<Client>();
await supabase.InitializeAsync();
supabase.Auth.LoadSession();

await app.RunAsync();