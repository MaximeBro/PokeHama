using System.ComponentModel.DataAnnotations;

namespace PokeHama.Models.Account;

public class UserCollection
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(50)]
    public string Username { get; set; } = null!;
    public int PokemonId { get; set; }
}