using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PokeHama.Services;

namespace PokeHama.Components.Pages;

public partial class Home
{
    [Inject] public FetchService FetchService { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;

    private List<string> _pokemons = new();
    private bool _loading;
    private int _amountToDisplay = 151;


    public int AmountToDisplay
    {
        get => _amountToDisplay;
        set
        {
            _amountToDisplay = value;
            _pokemons = FetchService.Pokemons.Select(x => x.Value.Img).Take(value).ToList();
             StateHasChanged();
        }
    }

    protected override void OnInitialized()
    {
        _loading = true;
        _pokemons = FetchService.Pokemons.Select(x => x.Value.Img).ToList();
        _loading = false;
    }
}