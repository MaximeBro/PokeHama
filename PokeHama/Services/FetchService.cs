using System.Text.Json;
using Microsoft.JSInterop;
using PokeHama.Extensions;
using PokeHama.Models;

namespace PokeHama.Services;

public class FetchService
{
    public readonly Dictionary<int, PokemonData> Pokemons = new();
    public Dictionary<int, string> Names = new();

    private readonly HttpClient _client = new();

    public FetchService() { }

    public Task InitAsync()
    {
        var stream = File.OpenRead($"{Directory.GetCurrentDirectory()}\\Data\\pokemon_names.json");
        Names = JsonSerializer.Deserialize<Dictionary<int, string>>(stream)!;
        for (int i = 1; i <= Hardcoded.IndexMax; i++)
        {
            
            Pokemons.Add(i, new PokemonData
            {
                Id = i,
                Name = Names[i]
            });
        }

        return Task.CompletedTask;
    }

    public async Task<List<DotNetStreamReference>> GetPokemonsAsStreamsAsync(int amount = 807)
    {
        var streams = await GetImageStreamAsync(amount);
        return streams.Select(x => new DotNetStreamReference(x)).ToList();
    }

    private async Task<List<Stream>> GetImageStreamAsync(int amount)
    {
        var streams = await Task.WhenAll(Pokemons.Take(amount).ToList()
            .ConvertAll(async x => await _client.GetStreamAsync(x.Value.Img)));
        return streams.ToList();
    }
}