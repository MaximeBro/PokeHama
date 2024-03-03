using Microsoft.EntityFrameworkCore;
using PokeHama.Models;

namespace PokeHama.Databases;

public class UtilityContext : DbContext
{
    public DbSet<UserModel> Users { get; set; }

    public UtilityContext(DbContextOptions<UtilityContext> options) : base(options) { }
}