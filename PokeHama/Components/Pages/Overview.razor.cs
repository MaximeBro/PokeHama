using System.Drawing;
using Microsoft.AspNetCore.Components;
using SkiaSharp;

namespace PokeHama.Components.Pages;

public partial class Overview
{
	[Parameter]
	public int Id { get; set; }
	
	private static readonly string _default_url = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vii/icons/";

	private readonly Dictionary<int, KeyValuePair<int, string>> _pixelMap = new ();
	private readonly List<string> _pixels = new();
	private SKBitmap _image = null!;

	protected override async Task OnInitializedAsync()
	{
		var client = new HttpClient();
		var stream = await client.GetStreamAsync($"{_default_url}{Id}.png");
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
				_pixels.Add($"{color.R:X2}{color.G:X2}{color.B:X2}");
			}
		}
	}
}