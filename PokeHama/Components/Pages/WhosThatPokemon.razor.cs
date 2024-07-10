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
    [Inject] public MiniGamesService MiniGamesService { get; set; } = null!;
    [Inject] public FetchService FetchService { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Parameter] public int? Id { get; set; }
    
    private readonly List<string> _pixels = new();
    private List<string> _colors = new();
    private SKBitmap _image = null!;

    private int _currentId = -1;
    private string _guess = string.Empty;
    private string BlinkClass => _successOver ? "transparent" : "blink";
    private bool _successOver;
    private bool _displayMute;

    protected override async Task OnInitializedAsync()
    {
        _currentId = new Random().Next(1, 808);
        if (Id.HasValue)
        {
            _currentId = Id.Value;
        }
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
            _colors.Add(pixel != "#000000" ? "rgba(120, 120, 120, .05)" : "#000000");
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
                    if (_guess.ToAscii() == "pikachu")
                    {
                        _guess = string.Empty;
                        await RevealPokemonAsync();
                        await PlayItsPikachuAsync();
                        _successOver = true;
                        Snackbar.Add("<span class=\"text-center\">Bien joué !</span>", Severity.Success, options =>
                        {
                            options.HideIcon = true;
                            options.SnackbarVariant = Variant.Text;
                        });
                        return;
                    }
                }
                else
                {
                    if (comparer.Compare(_guess.ToAscii(), answer.ToAscii()) == 0)
                    {
                        _guess = string.Empty;
                        await RevealPokemonAsync();
                        _successOver = true;
                        Snackbar.Add("Bien joué !", Severity.Info, options =>
                        {
                            options.HideIcon = true;
                            options.SnackbarVariant = Variant.Text;
                        });
                        return;
                    }
                }
                
                _guess = string.Empty;
                Snackbar.Add(MiniGamesService.GetRandomFailGuessMessage(), Severity.Warning);
                await PlayRickRollAsync();
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
        _displayMute = true;
        StateHasChanged();
    }
    
    private async Task PlayItsPikachuAsync()
    {
        await JsRuntime.InvokeVoidAsync("playItsPikachu");
        _displayMute = true;
        StateHasChanged();
    }

    private async Task StopVideosAsync()
    {
        await JsRuntime.InvokeVoidAsync("stopRickRoll");
        await JsRuntime.InvokeVoidAsync("stopItsPikachu");
        _displayMute = false;
        StateHasChanged();
    }

    [JSInvokable]
    private Task ToggleMuteButton()
    {
        _displayMute = false;
        StateHasChanged();
        return Task.CompletedTask;
    }
}