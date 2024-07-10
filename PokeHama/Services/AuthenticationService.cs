using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using PokeHama.Databases;
using BC = BCrypt.Net.BCrypt;

namespace PokeHama.Services;

public class AuthenticationService(IDbContextFactory<UtilityContext> factory, UserTokenService userTokenService, NavigationManager navManager)
{
	public async Task<bool> ValidateLoginAsync(string username, string password)
	{
		await using var db = await factory.CreateDbContextAsync();
		var users = await db.Users.ToListAsync();
		var user = users.FirstOrDefault(x => x.Username == username && BC.Verify(password, x.Password));
		if (user != null)
		{
			var token = Guid.NewGuid();
			Claim[] claims = new Claim[]
			{
				new Claim(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}"),
				new Claim(ClaimTypes.Name, user.Username),
				new Claim(ClaimTypes.Role, user.Role.ToString())
			};
			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			
			userTokenService.Tokens.Add(token, new ClaimsPrincipal(identity));
			navManager.NavigateTo($"/api/authentication/login/{token}", true);
			return true;
		}

		return false;
	}
}