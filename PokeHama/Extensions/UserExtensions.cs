using Microsoft.EntityFrameworkCore;
using PokeHama.Models.Relationships;
using PokeHama.Models.Account.Enums;

namespace PokeHama.Extensions;

public static class UserExtensions
{
    public static bool IsPublic(this AccountPrivacy @this)
    {
        return @this is AccountPrivacy.Public or AccountPrivacy.AdminPublic;
    }

    public static bool AreFriends(this DbSet<UserRelationship> @this, string from, string to)
    {
        return @this.AsNoTracking().FirstOrDefault(x => 
                    (x.Username == from && x.FriendUsername == to) || 
                    (x.Username == to && x.FriendUsername == from)) != null;
    }
}