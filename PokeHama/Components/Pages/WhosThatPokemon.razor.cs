using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using PokeHama.Extensions;
using PokeHama.Services;
using SkiaSharp;
using Color = System.Drawing.Color;

namespace PokeHama.Components.Pages;

public partial class WhosThatPokemon
{
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [Inject] public FetchService FetchService { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    
    private readonly List<string> _pixels = new();
    private List<string> _colors = new();
    private SKBitmap _image = null!;

    private int _currentId = -1;
    private string _guess = string.Empty;
    private string _blinkClass => _successOver ? "transparent" : "blink";
    private bool _successOver;

    protected override async Task OnInitializedAsync()
    {
        _currentId = new Random().Next(1, 808);
        var client = new HttpClient();
        var stream = await client.GetStreamAsync($"{Hardcoded.IconUrl}{_currentId}.png");
        var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        ms.Position = 0;
        _image = SKBitmap.Decode(new SKManagedStream(ms));
        for (int y = 0; y < _image.Height; y++)
        {
            for (int x = 0; x < _image.Width; x++)
            {
                var pixel = _image.GetPixel(x, y);
                int r = pixel.Red;
                int g = pixel.Green;
                int b = pixel.Blue;
                var color = Color.FromArgb(r, g, b);
                _pixels.Add($"#{color.R:X2}{color.G:X2}{color.B:X2}");
            }
        }
        
        foreach (var pixel in _pixels)
        {
            if (pixel != "#000000")
            {
                _colors.Add("rgba(120, 120, 120, .05)");
            }
            else
            {
                _colors.Add("#000000");
            }
        }
    }

    private async Task TryGuessNameAsync()
    {
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;
        Snackbar.Configuration.VisibleStateDuration = 1500;
        Snackbar.Configuration.ShowCloseIcon = false;
        Snackbar.Configuration.HideTransitionDuration = 100;
        
        if (FetchService.Names.TryGetValue(_currentId, out var answer))
        {
            if (!string.IsNullOrEmpty(_guess))
            {
                var comparer = StringComparer.Create(new CultureInfo("fr-FR"), true);
                if (_currentId == 35) // EASTER w/ Mélofée
                {
                    if (comparer.Compare(_guess, "pikachu") == 0)
                    {
                        _guess = string.Empty;
                        await RevealPokemonAsync();
                        await PlayItsPikachuAsync();
                        StateHasChanged();
                        _successOver = true;
                        Snackbar.Add("<span class=\"text-center\">Bien joué !</span>", Severity.Success, options =>
                        {
                            options.HideIcon = true;
                            options.SnackbarVariant = Variant.Text;
                        });
                    }
                    else
                    {
                        _guess = string.Empty;
                        Snackbar.Add("Non, ce n'est pas ça...Essaye de nouveau !", Severity.Warning);
                        await PlayRickRollAsync();
                    }
                }
                else
                {
                    if (comparer.Compare(_guess.Replace("é", "e").Replace("è", "e"), answer.Replace("é", "e").Replace("è", "e")) == 0)
                    {
                        _guess = string.Empty;
                        _successOver = true;
                        Snackbar.Add("Bien joué !", Severity.Info, options =>
                        {
                            options.HideIcon = true;
                            options.SnackbarVariant = Variant.Text;
                        });
                    }
                    else
                    {
                        _guess = string.Empty;
                        Snackbar.Add("Non, ce n'est pas ça...Essaye de nouveau !", Severity.Warning);
                        await PlayRickRollAsync();
                    } 
                }
            }
        }
        else
        {
            Snackbar.Add("Un problème est survenu...", Severity.Error);
        }
    }

    private void RefreshGame() => NavManager.NavigateTo("/guess", true);
    private void LeaveGame() => NavManager.NavigateTo("/", true);

    private async Task RevealPokemonAsync()
    {
        _pixels.Clear();
        var client = new HttpClient();
        var stream = await client.GetStreamAsync($"{Hardcoded.IconUrl}{_currentId}.png");
        var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        ms.Position = 0;
        _image = SKBitmap.Decode(new SKManagedStream(ms));
        for (int y = 0; y < _image.Height; y++)
        {
            for (int x = 0; x < _image.Width; x++)
            {
                var pixel = _image.GetPixel(x, y);
                int r = pixel.Red;
                int g = pixel.Green;
                int b = pixel.Blue;
                var color = Color.FromArgb(r, g, b);
                _pixels.Add($"#{color.R:X2}{color.G:X2}{color.B:X2}");
            }
        }

        _colors = _pixels.ToList();
    }

    private async Task PlayRickRollAsync()
    {
        await JsRuntime.InvokeVoidAsync("playRickRoll");
    }
    
    private async Task PlayItsPikachuAsync()
    {
        await JsRuntime.InvokeVoidAsync("playItsPikachu");
    }
}