using System.ComponentModel.DataAnnotations;
using PokeHama.Models.Account.Enums;

namespace PokeHama.Models.Account;

public class UserModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAd { get; set; }
}