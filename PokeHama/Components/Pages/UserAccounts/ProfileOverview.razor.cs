using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using PokeHama.Databases;
using PokeHama.Models.Account;
using PokeHama.Models.Account.Enums;
using PokeHama.Services;

namespace PokeHama.Components.Pages.UserAccounts;

public partial class ProfileOverview
{
    [Inject] public IDbContextFactory<UtilityContext> UtilityFactory { get; set; } = null!;
    [Inject] public FetchService FetchService { get; set; } = null!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;
    [Parameter] public string? Username { get; set; }
    
    private UserModel? _user;
    private UserData? _data;
    private Dictionary<int, string> _collection = [];
    private string? _currentUser;

    private bool _isPublic => _data?.AccountPrivacy == AccountPrivacy.Public || _data?.AccountPrivacy == AccountPrivacy.AdminPublic;
    private string _isPublicText => _isPublic ? "public" : "privé";

    protected override async Task OnInitializedAsync()
    {
        var utilityDb = await UtilityFactory.CreateDbContextAsync();
        await RefreshCurrentUserAsync();
        _user = utilityDb.Users.FirstOrDefault(x => x.Username == Username);
        _data = utilityDb.UsersData.FirstOrDefault(x => x.Username == Username);
        var collection = utilityDb.UsersCollections.AsNoTracking().Where(x => x.Username == Username).Select(x => x.PokemonId).ToList();
        _collection = FetchService.Pokemons.Where(x => collection.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value.Img);

        if (_user is null || _data is null)
        {
            NavManager.NavigateTo("/", true);
        }
    }

    private async Task CopyToClipboardAsync()
    {
        await JsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", $"pokehama.fr/profile-overview/{Username}");
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
        Snackbar.Add("Lien copié dans le presse papier.", Severity.Success);
    }

    private async Task AddFriendAsync()
    {
        await RefreshCurrentUserAsync();
        if (_currentUser != null && _currentUser != Username)
        {
            
        }
    }

    private async Task RemoveFriendAsync()
    {
        await RefreshCurrentUserAsync();
        if (_currentUser != null && _currentUser != Username)
        {
            
        }
    }

    private async Task RefreshCurrentUserAsync()
    {
        var session = await AuthenticationStateTask;
        _currentUser = session.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
    }
}