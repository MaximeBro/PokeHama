using Microsoft.EntityFrameworkCore;
using PokeHama.Databases;
using PokeHama.Extensions;
using PokeHama.Models.Account;
using PokeHama.Models.Relationships;

namespace PokeHama.Services;

public class RelationshipManager
{
    private readonly IDbContextFactory<UtilityContext> _utilityFactory;
    
    public RelationshipManager(IDbContextFactory<UtilityContext> utilityFactory)
    {
        _utilityFactory = utilityFactory;
    }

    public async Task<bool> CanSendInviteAsync(string from, string to)
    {
        var utilityDb = await _utilityFactory.CreateDbContextAsync();
        var areFriends = utilityDb.UsersRelationships.AreFriends(from, to);
        var alreadySent = utilityDb.PendingInvites.AsNoTracking().FirstOrDefault(x => x.From == from && x.To == to);
        return !areFriends && alreadySent is null;
    }
    
    public async Task SendInviteAsync(string from, string to)
    {
        var utilityDb = await _utilityFactory.CreateDbContextAsync();
        utilityDb.PendingInvites.Add(new PendingInvite { From = from, To = to });
        await utilityDb.SaveChangesAsync();
        await utilityDb.DisposeAsync();
    }

    public async Task AcceptFriendRequestAsync(string from, string to)
    {
        var utilityDb = await _utilityFactory.CreateDbContextAsync();
        var request = utilityDb.PendingInvites.AsNoTracking().FirstOrDefault(x => x.From == from && x.To == to);
        var areFriends = utilityDb.UsersRelationships.AreFriends(from, to);
        
        if (request != null && !areFriends)
        {
            utilityDb.PendingInvites.Remove(request);
            utilityDb.UsersRelationships.Add(new UserRelationship { Username = from, FriendUsername = to });
            await utilityDb.SaveChangesAsync();
        }

        await utilityDb.DisposeAsync();
    }

    public async Task DenyFriendRequestAsync(string from, string to)
    {
        var utilityDb = await _utilityFactory.CreateDbContextAsync();
        var request = utilityDb.PendingInvites.AsNoTracking().FirstOrDefault(x => x.From == from && x.To == to);
        if (request != null)
        {
            utilityDb.PendingInvites.Remove(request);
            await utilityDb.SaveChangesAsync();
        }

        await utilityDb.DisposeAsync();
    }

    public async Task RemoveFriendAsync(string from, string to)
    {
        var utilityDb = await _utilityFactory.CreateDbContextAsync();
        if (utilityDb.UsersRelationships.AreFriends(from, to))
        {
            var relationships = await utilityDb.UsersRelationships.Where(x => 
                                                                (x.Username == from && x.FriendUsername == to) || 
                                                                (x.Username == to && x.FriendUsername == from)).ToListAsync();
            utilityDb.UsersRelationships.RemoveRange(relationships);
            await utilityDb.SaveChangesAsync();
        }

        await utilityDb.DisposeAsync();
    }

    public async Task<List<UserData>> GetFriendsAsync(string username)
    {
        var utilityDb = await _utilityFactory.CreateDbContextAsync();
        var firstGroup = await utilityDb.UsersRelationships.AsNoTracking()
                                                                     .Where(x => x.Username == username)
                                                                     .Select(x => x.FriendUsername)
                                                                     .ToListAsync();

        var secondGroup = await utilityDb.UsersRelationships.AsNoTracking()
                                                                      .Where(x => x.FriendUsername == username)
                                                                      .Select(x => x.Username)
                                                                      .ToListAsync();

        var friends = await utilityDb.UsersData.AsNoTracking()
                                                            .Where(x => firstGroup.Contains(x.Username) || 
                                                                                secondGroup.Contains(x.Username))
                                                            .ToListAsync();

        return friends;
    }

    public async Task<List<UserData>> GetPendingInvitesAsync(string username)
    {
        var utilityDb = await _utilityFactory.CreateDbContextAsync();
        var invites = await utilityDb.PendingInvites.AsNoTracking()
                                                              .Where(x => x.To == username)
                                                              .Distinct()
                                                              .Select(x => x.From)
                                                              .ToListAsync();
        var users = await utilityDb.UsersData.AsNoTracking()
                                                         .Where(x => invites.Contains(x.Username))
                                                         .ToListAsync();
        return users;
    }
}