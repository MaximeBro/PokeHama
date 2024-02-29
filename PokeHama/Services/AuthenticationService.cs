using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using PokeHama.Databases;
using PokeHama.Models;

namespace PokeHama.Services;

public class AuthenticationService
{
    private readonly IDbContextFactory<UtiliyContext> _utilityFactory;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly HttpContext _httpContext;
    
    public delegate Task SignOutEventHandler();
    public event SignOutEventHandler? OnSignOutAsync;
    
    public ClaimsPrincipal? User { get; private set; }
    public bool IsAuthenticated => User != null;

    public AuthenticationService(IDbContextFactory<UtiliyContext> utilityFactory, AuthenticationStateProvider authenticationStateProvider, HttpContextAccessor httpContext)
    {
        _utilityFactory = utilityFactory;
        _authenticationStateProvider = authenticationStateProvider;
        _httpContext = httpContext.HttpContext!;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User.Identity != null && authState.User.Identity.IsAuthenticated;
    }

    public async Task SignInAsync(UserModel userModel)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userModel.UserName),
            new Claim("firstname", userModel.FirstName),
            new Claim("lastname", userModel.LastName),
            new Claim(ClaimTypes.Role, userModel.Role.ToString())
        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties { IsPersistent = true, RedirectUri = "/login" };
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        User = claimsPrincipal;
        await _httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
    }

    public async Task SignOutAsync()
    {
        var authenticated = await IsAuthenticatedAsync();
        if (authenticated) await _httpContext.SignOutAsync();
        User = null;
        if (OnSignOutAsync != null)
        {
            await OnSignOutAsync.Invoke();
        }
    }
    
    public async Task<UserModel?> GetUserAsync()
    {
        if (User is null) return null;
        
        var utilityDb = await _utilityFactory.CreateDbContextAsync();
        var user = await utilityDb.Users.FirstOrDefaultAsync(x => x.UserName == User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)!.Value);

        return user;
    }
}