using Microsoft.EntityFrameworkCore;
using PokeHama.Models.Account;
using PokeHama.Models.Relationships;

namespace PokeHama.Databases;

public class UtilityContext : DbContext
{
    public DbSet<UserModel> Users { get; set; }
    public DbSet<UserData> UsersData { get; set; }
    public DbSet<UserCollection> UsersCollections { get; set; }
    public DbSet<UserRelationship> UsersRelationships { get; set; }
    public DbSet<PendingInvite> PendingInvites { get; set; }

    public UtilityContext(DbContextOptions<UtilityContext> options) : base(options) { }
}