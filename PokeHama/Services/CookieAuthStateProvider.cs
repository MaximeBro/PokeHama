using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

namespace PokeHama.Services;

public class CookieAuthStateProvider : RevalidatingServerAuthenticationStateProvider
{
    public CookieAuthStateProvider(ILoggerFactory loggerFactory) : base(loggerFactory) { }

    protected override Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);
}