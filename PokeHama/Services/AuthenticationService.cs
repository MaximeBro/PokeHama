using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using PokeHama.Databases;
using PokeHama.Models.Account;
using PokeHama.Models.Account.Enums;
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
				new Claim("FirstName", user.FirstName),
				new Claim("LastName", user.LastName),
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

	public async Task<UserModel?> GetUserAsync(Task<AuthenticationState> task)
	{
		var state = await task;
		if (state.User is {Identity: {IsAuthenticated: true}})
		{
			return new UserModel
			{
				Username = state.User.FindFirstValue(ClaimTypes.Name)!,
				FirstName = state.User.FindFirstValue("FirstName")!,
				LastName = state.User.FindFirstValue("LastName")!,
				Role = Enum.Parse<UserRole>(state.User.FindFirstValue(ClaimTypes.Role)!)
			};
		}

		return null;
	}
}