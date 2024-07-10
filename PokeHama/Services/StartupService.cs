using Microsoft.EntityFrameworkCore;
using PokeHama.Databases;
using PokeHama.Models.Account.Enums;

namespace PokeHama.Services;

public class StartupService(IConfiguration configuration, IDbContextFactory<UtilityContext> factory)
{
	public async Task InitAsync()
	{
		await CheckUsersAsync();
	}

	private async Task CheckUsersAsync()
	{
		await using var db = await factory.CreateDbContextAsync();
		foreach (var user in configuration.GetSection("users").GetChildren())
		{
			var username = user.GetSection("Username").Value ?? string.Empty;
			var role = user.GetSection("Role").Value ?? "User";

			var old = await db.Users.FirstOrDefaultAsync(x => x.Username == username);
			if (old != null && Enum.TryParse<UserRole>(role, out var newRole))
			{
				old.Role = newRole;
				await db.SaveChangesAsync();
				
				Console.WriteLine($"[StartupService] User {username} role has been updated to {newRole}.");
			}
		}
	}
	
}