using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;

namespace SupasharpTodo.Shared.Providers;

/// <summary>
/// Creates a link between <see cref="Supabase"/> and <see cref="AuthenticatedState"/> to provide support for using
/// Gotrue with Blazor's built in Authentication handler.
/// </summary>
public class SupabaseAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
{
    private Supabase.Client Supabase { get; }

    private AuthenticationState AnonymousState => new(new ClaimsPrincipal(new ClaimsIdentity()));

    /// <summary>
    /// Creates an <see cref="AuthenticationState"/> that is either Anonymous or Authenticated if Gotrue has a current user.
    /// </summary>
    private AuthenticationState AuthenticatedState
    {
        get
        {
            var user = Supabase.Auth.CurrentUser;

            if (user == null)
                return AnonymousState;

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.Role, user.Role!),
                new(ClaimTypes.Authentication, "supabase")
            };

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "supabase")));
        }
    }

    public SupabaseAuthenticationStateProvider(Supabase.Client supabase)
    {
        Console.WriteLine($"{nameof(SupabaseAuthenticationStateProvider)} initialized.");
        Supabase = supabase;
        Supabase.Auth.AddStateChangedListener(SupabaseAuthStateChanged);
    }

    public void Dispose()
    {
        Supabase.Auth.RemoveStateChangedListener(SupabaseAuthStateChanged);
    }

    /// <summary>
    /// Adds a listener on the supabase client that will notify components of a change in authentication state in realtime.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="state"></param>
    private void SupabaseAuthStateChanged(IGotrueClient<User, Session> sender, Constants.AuthState state)
    {
        switch (state)
        {
            case Constants.AuthState.SignedIn:
                NotifyAuthenticationStateChanged(Task.FromResult(AuthenticatedState));
                break;
            case Constants.AuthState.SignedOut:
                NotifyAuthenticationStateChanged(Task.FromResult(AnonymousState));
                break;
        }
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        Supabase.Auth.LoadSession();

        if (Supabase.Auth.CurrentUser == null)
        {
            Console.WriteLine("An authenticated user not found, returning as anonymous.");
            return Task.FromResult(AnonymousState);
        }

        return Task.FromResult(AuthenticatedState);
    }
}