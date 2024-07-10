using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using PokeHama.Databases;
using PokeHama.Extensions;
using PokeHama.Models.Account;
using PokeHama.Services;
using SkiaSharp;
using Color = System.Drawing.Color;

namespace PokeHama.Components.Pages;

public partial class Overview
{
    [Inject] public IDbContextFactory<UtilityContext> UtilityFactory { get; set; } = null!;
    [Inject] public FetchService FetchService { get; set; } = null!;
    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;
    [Parameter] public int Id { get; set; }

    private readonly List<string> _pixels = [];
    private List<string> _palette = [];
    private SKBitmap _image = null!;
    private string _name = string.Empty;

    private bool _isConnected;
    private bool _canAddToCollection;
    
    private bool _toggleGrid = true;
    private bool _toggleLegend;
    private bool _toggleFullScreen;
    private int _pixelSize = 25;
    private string _gridStyle => _toggleGrid ? "border: thin solid #424242;" : string.Empty;
    private string _scaleStyle => _toggleGrid ? "scalable" : string.Empty;
    private int _headerColspan => _toggleLegend ? 41 : 40;

    protected override async Task OnInitializedAsync()
    {
        await RefreshDataAsync();
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

    private async Task CopyToClipboardAsync(string color)
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
    
    private async Task DownloadPngAsync()
    {
        var client = new HttpClient();
        var fileStream = await client.GetStreamAsync($"{Hardcoded.IconUrl}{Id}.png");
        var dotnetStream = new DotNetStreamReference(fileStream);
        
        await JsRuntime.InvokeVoidAsync("downloadFileFromStream", $"{_name}.png", dotnetStream);
    }
    
    private async Task AddToCollectionAsync()
    {
        var utilityDb = await UtilityFactory.CreateDbContextAsync();
        var authenticationState = await AuthenticationStateTask;
        var username = authenticationState.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? string.Empty;
        var user = utilityDb.Users.FirstOrDefault(x => x.Username == username);

        if (user != null)
        {
            utilityDb.UsersCollections.Add(new UserCollection { Username = user.Username, PokemonId = Id });
            await utilityDb.SaveChangesAsync();
        }

        await utilityDb.DisposeAsync();
        await RefreshDataAsync();
    }

    private async Task RemoveFromCollectionAsync()
    {
        var utilityDb = await UtilityFactory.CreateDbContextAsync();
        var authenticationState = await AuthenticationStateTask;
        var username = authenticationState.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? string.Empty;
        var user = utilityDb.Users.FirstOrDefault(x => x.Username == username);

        if (user != null)
        {
            var collectionItem = utilityDb.UsersCollections.FirstOrDefault(x => x.Username == user.Username && x.PokemonId == Id);
            if (collectionItem != null)
            {
                utilityDb.Remove(collectionItem);
                await utilityDb.SaveChangesAsync();
            }
        }
        
        await utilityDb.DisposeAsync();
        await RefreshDataAsync();
    }

    private async Task RefreshDataAsync()
    {
        var utilityDb = await UtilityFactory.CreateDbContextAsync();
        var authenticationState = await AuthenticationStateTask;
        var username = authenticationState.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? string.Empty;
        var user = utilityDb.Users.AsNoTracking().FirstOrDefault(x => x.Username == username);
        if (user != null)
        {
            _isConnected = true;
            var alreadySaved = utilityDb.UsersCollections.AsNoTracking().FirstOrDefault(x => x.Username == user.Username && x.PokemonId == Id);
            _canAddToCollection = alreadySaved is null;
        }

        await utilityDb.DisposeAsync();
    }
}