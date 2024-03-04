using PokeHama.Models.Account.Enums;

namespace PokeHama.Extensions;

public static class UserExtensions
{
    public static bool IsPublic(this AccountPrivacy @this)
    {
        return @this is AccountPrivacy.Public or AccountPrivacy.AdminPublic;
    }
}