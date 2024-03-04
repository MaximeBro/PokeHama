using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace PokeHama.Controllers;

public class AuthenticateController : Controller
{
    [HttpGet("/api/authenticate/login")]
    public async Task<IActionResult> OnLoginAsync(string username, string role)
    {
        var claims = new []
        {
            new Claim(ClaimTypes.Surname, username),
            new Claim(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties { AllowRefresh = true, IsPersistent = true };

        try
        {
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProperties);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        return Redirect("/");
    }

    [HttpGet("/api/authenticate/logout")]
    public async Task<IActionResult> OnLogoutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/connexion");
    }
}