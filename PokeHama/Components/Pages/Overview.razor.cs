using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using PokeHama.Extensions;
using PokeHama.Services;
using SkiaSharp;
using Color = System.Drawing.Color;

namespace PokeHama.Components.Pages;

public partial class Overview
{
    [Inject] public FetchService FetchService { get; set; } = null!;
    [Parameter] public int Id { get; set; }

    private readonly List<string> _pixels = new();
    private List<string> _palette = new();
    private SKBitmap _image = null!;
    private string _name = string.Empty;

    private bool _toggleGrid = true;
    private bool _toggleFullScreen = false;
    private int _pixelSize = 25;
    private string _gridStyle => _toggleGrid ? "border: thin solid #424242;" : string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var client = new HttpClient();
        var stream = await client.GetStreamAsync($"{Hardcoded.IconUrl}{Id}.png");
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

        _palette = _pixels.Distinct().ToList();
        _name = FetchService.Names[Id];
    }

    public async Task CopyToClipboardAsync(string color)
    {
        await JsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", color);
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
        Snackbar.Configuration.PreventDuplicates = false;
        Snackbar.Configuration.MaxDisplayedSnackbars = Int32.MaxValue;
        Snackbar.Configuration.VisibleStateDuration = 1500;
        Snackbar.Add($"[{color}] Couleur enregistrée dans le presse papier.", Severity.Info);
    }

    public async Task ToggleFullScreen(bool toggle)
    {
        if (toggle) await JsRuntime.OpenFullscreen();
        else await JsRuntime.CloseFullscreen();
        
        _toggleFullScreen = toggle;
        StateHasChanged();
    }
}