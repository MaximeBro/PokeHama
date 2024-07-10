using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PokeHama.Services;

namespace PokeHama.Extensions;

public static class ServicesExtensions
{
    public static void UseUploading(this WebApplication @this)
    {
        @this.MapGet("/api/files/{name:required}", (HttpResponse response, string name) =>
        {
            response.Headers.ContentDisposition = "inline";
            var stream = new StreamReader($"wwwroot/_content/pictures/{name}");
            return Results.Stream(stream.BaseStream, MimeTypes.GetMimeTypeOf(Path.GetExtension(name)));
        });
    }

    public static void UseLogin(this WebApplication @this)
    {
        @this.Map("/api/authentication/login/{token:guid}", async (HttpContext context, Guid token, UserTokenService userTokenService) =>
        {
            if (userTokenService.Tokens.TryGetValue(token, out var claims))
            {
                var authProperties = new AuthenticationProperties { AllowRefresh = true, IsPersistent = true };
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claims, authProperties);
                return Results.Redirect("/");
            }

            return Results.Unauthorized();
        });
    }
    
    public static void UseLogout(this WebApplication @this)
    {
        @this.Map("/api/authentication/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/connexion");
        });
    }
}