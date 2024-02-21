using System.Drawing;
using MudBlazor.Utilities;
using PokeHama.Extensions;
using SkiaSharp;

namespace PokeHama.Components.Pages;

public partial class WhosThatPokemon
{
    private readonly Dictionary<int, KeyValuePair<int, string>> _pixelMap = new ();
    private readonly List<string> _pixels = new();
    private List<string> _colors = new();
    private SKBitmap _image = null!;


    protected override async Task OnInitializedAsync()
    {
        var id = new Random().Next(1, 808);
        var client = new HttpClient();
        var stream = await client.GetStreamAsync($"{Hardcoded.IconUrl}{id}.png");
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
                _colors.Add("#424242");
            }
            else
            {
                _colors.Add(pixel);
            }
        }
    }
}