using System.ComponentModel.DataAnnotations;
using PokeHama.Models.Account.Enums;

namespace PokeHama.Models.Account;

public class UserData
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [MaxLength(50)]
    public string Username { get; set; } = null!;

    [MaxLength(10)] 
    public string Pfp { get; set; } = null!;

    public bool ImagePfp { get; set; }
    
    public AccountPrivacy AccountPrivacy { get; set; } = AccountPrivacy.Public;
}