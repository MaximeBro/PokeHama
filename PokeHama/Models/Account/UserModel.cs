using System.ComponentModel.DataAnnotations;
using PokeHama.Models.Account.Enums;

namespace PokeHama.Models.Account;

public class UserModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(50)]
    public string Username { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public UserRole Role { get; set; }
    public DateTime CreatedAd { get; set; }
}