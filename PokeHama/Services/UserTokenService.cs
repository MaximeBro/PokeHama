using System.Security.Claims;

namespace PokeHama.Services;

public class UserTokenService
{
	public Dictionary<Guid, ClaimsPrincipal> Tokens { get; set; } = [];
}