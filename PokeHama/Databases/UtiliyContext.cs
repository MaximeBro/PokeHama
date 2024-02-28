using Microsoft.EntityFrameworkCore;
using PokeHama.Models;

namespace PokeHama.Databases;

public class UtiliyContext : DbContext
{
    public DbSet<UserModel> Users { get; set; }

    public UtiliyContext(DbContextOptions<UtiliyContext> options) : base(options) { }
}