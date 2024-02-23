using PokeHama.Extensions;

namespace PokeHama.Models;

public class PokemonData
{
    public int Id { get; set; }
    public string Img => $"{Hardcoded.IconUrl}{Id}.png";
    public string? Name { get; set; }
}