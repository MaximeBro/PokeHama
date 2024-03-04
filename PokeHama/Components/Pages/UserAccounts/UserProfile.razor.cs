using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using PokeHama.Components.Dialogs;
using PokeHama.Databases;
using PokeHama.Extensions;
using PokeHama.Models.Account;
using PokeHama.Models.Account.Enums;
using BC = BCrypt.Net.BCrypt;

namespace PokeHama.Components.Pages.UserAccounts;

public partial class UserProfile
{
    [Inject] public IDbContextFactory<UtilityContext> UtilityFactory { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;
    public int SelectedTab { get; set; } = 0;

    private UserModel? _user;
    private UserData? _data;

    private AccountPrivacy _accountPrivacy;
    private bool IsPublic => _accountPrivacy.IsPublic();
    private string PrivacyText => IsPublic ? "public" : "privé";

    private DialogOptions _dialogOptions = new()
    {
        DisableBackdropClick = false,
        CloseOnEscapeKey = true
    };
    
    protected override async Task OnInitializedAsync()
    {
        await RefreshDataAsync();
    }
    
    private async Task EditUsernameAsync()
    {
        var utilityDb = await UtilityFactory.CreateDbContextAsync();
        var usernames = utilityDb.Users.AsTracking().Select(x => x.Username).ToList();
        usernames.Remove(_user!.Username);
        var parameters = new DialogParameters<EditUsername> { { x => x.Username, _user!.Username }, { x => x.Usernames, usernames } };
        var dialog = await DialogService.ShowAsync<EditUsername>(string.Empty, parameters, _dialogOptions);
        var result = await dialog.Result;
        if (result is { Data: string username })
        {
            var oldUser = utilityDb.Users.AsTracking().FirstOrDefault(x => x.Username == _user!.Username);
            if (oldUser != null)
            {
                oldUser.Username = username;
                await utilityDb.SaveChangesAsync();
            }
            await utilityDb.DisposeAsync();
            NavManager.NavigateTo("api/authenticate/logout", true);
        }
    }

    private async Task EditPfpAsync()
    {
        var utilityDb = await UtilityFactory.CreateDbContextAsync(); 
    }
    
    private async Task EditRelativeNameAsync()
    {
        var utilityDb = await UtilityFactory.CreateDbContextAsync();
        var parameters = new DialogParameters<EditRelativeName> { { x => x.Firstname, _user!.FirstName }, { x => x.Lastname, _user!.LastName } };
        var dialog = await DialogService.ShowAsync<EditRelativeName>(string.Empty, parameters, _dialogOptions);
        var result = await dialog.Result;
        if (result is { Data: Dictionary<string, string> data })
        {
            var oldUser = utilityDb.Users.AsTracking().FirstOrDefault(x => x.Username == _user!.Username);
            if (oldUser != null)
            {
                oldUser.FirstName = data["firstname"];
                oldUser.LastName = data["lastname"];
                await utilityDb.SaveChangesAsync();
            }
            await utilityDb.DisposeAsync();
            await RefreshDataAsync();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;
            Snackbar.Add("Vos données personnelles ont bien été mises à jour.", Severity.Success, options =>
            {
                options.VisibleStateDuration = 2000;
                options.ShowCloseIcon = false;
            });
        }
    }

    private async Task EditPasswordAsync()
    {
        var utilityDb = await UtilityFactory.CreateDbContextAsync();
        var parameters = new DialogParameters<EditPassword> { { x => x.OldPasswordHash, _user!.Password } };
        var dialog = await DialogService.ShowAsync<EditPassword>(string.Empty, parameters, _dialogOptions);
        var result = await dialog.Result;
        if (result is { Data: string password })
        {
            var oldUser = utilityDb.Users.AsTracking().FirstOrDefault(x => x.Username == _user!.Username);
            if (oldUser != null)
            {
                oldUser.Password = BC.HashPassword(password);
                await utilityDb.SaveChangesAsync();
            }
            await utilityDb.DisposeAsync();
            NavManager.NavigateTo("api/authenticate/logout", true);
        }
    }
    
    private async Task EditAccountPrivacyAsync(AccountPrivacy value)
    {
        var utilityDb = await UtilityFactory.CreateDbContextAsync();
        var data = utilityDb.UsersData.AsTracking().FirstOrDefault(x => x.Username == _user!.Username);
        if (data != null)
        {
            data.AccountPrivacy = value;
            await utilityDb.SaveChangesAsync();
            await utilityDb.DisposeAsync();
            await RefreshDataAsync(2);
        }
    }

    private async Task RefreshDataAsync(int tab = 0)
    {
        var utilityDb = await UtilityFactory.CreateDbContextAsync();
        var session = await AuthenticationStateTask;
        var username = session.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value ?? string.Empty;
        
        _user = utilityDb.Users.AsNoTracking().FirstOrDefault(x => x.Username == username);
        _data = utilityDb.UsersData.AsNoTracking().FirstOrDefault(x => x.Username == username);
        if (_user is null || _data is null)
        {
            NavManager.NavigateTo("/", true);
        }

        _accountPrivacy = _data!.AccountPrivacy;
        SelectedTab = tab;
    }
}