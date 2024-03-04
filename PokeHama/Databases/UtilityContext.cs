using Microsoft.EntityFrameworkCore;
using PokeHama.Models;
using PokeHama.Models.Account;

namespace PokeHama.Databases;

public class UtilityContext : DbContext
{
    public DbSet<UserModel> Users { get; set; }
    public DbSet<UserData> UsersData { get; set; }

    public UtilityContext(DbContextOptions<UtilityContext> options) : base(options) { }
}