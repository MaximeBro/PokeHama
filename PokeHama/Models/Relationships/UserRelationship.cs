using System.ComponentModel.DataAnnotations;

namespace PokeHama.Models.Account;

public class UserRelationship
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(50)]
    public string Username { get; set; } = null!;
    [MaxLength(50)]
    public string FriendUsername { get; set; } = null!;
}