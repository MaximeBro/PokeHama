using Microsoft.AspNetCore.Components.Authorization;
using PokeHama.Models;

namespace PokeHama.Services;

public class AuthenticationService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    
    public delegate Task SignInEventHandler();
    public event SignInEventHandler? OnSignInAsync;
    
    public delegate Task SignOutEventHandler();
    public event SignOutEventHandler? OnSignOutAsync;
    
    public UserModel? User { get; private set; }

    public AuthenticationService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User.Identity != null && authState.User.Identity.IsAuthenticated;
    }

    public async Task SignInAsync(UserModel user)
    {
        User = user;
        if (OnSignInAsync != null) await OnSignInAsync.Invoke();
    }

    public async Task SignOutAsync()
    {
        User = null;
        var authenticated = await IsAuthenticatedAsync();
        if (authenticated && OnSignOutAsync != null) await OnSignOutAsync.Invoke();
    }
}