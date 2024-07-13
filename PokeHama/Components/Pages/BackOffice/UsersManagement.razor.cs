using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using PokeHama.Components.Dialogs.BackOffice;
using PokeHama.Components.Dialogs.Common;
using PokeHama.Databases;
using PokeHama.Extensions;
using PokeHama.Models.Account;
using PokeHama.Services;
using BC = BCrypt.Net.BCrypt;

namespace PokeHama.Components.Pages.BackOffice;

public partial class UsersManagement
{
	[CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
	[Inject] public IDbContextFactory<UtilityContext> Factory { get; set; } = null!;
	[Inject] public AuthenticationService AuthenticationService { get; set; } = null!;
	[Inject] public IDialogService DialogService { get; set; } = null!;
	[Inject] public ISnackbar Snackbar { get; set; } = null!;
	[Inject] public NavigationManager NavManager { get; set; } = null!;

	private List<BreadcrumbItem> _breadcrumbs = [];
	private List<UserModel> _users = [];

	private UserModel _user = null!;

	private string _search = string.Empty;
	private Func<UserModel, bool> QuickFilter => x =>
	{
		if (x.Username.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
		if (x.FirstName.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
		if (x.LastName.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
		if (x.Role.ToString().Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
		if (x.CreatedAd.ToString("dd-MM-yyyy à HH:mm:ss").Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
		
		return false;
	};

	protected override async Task OnInitializedAsync()
	{
		_user = (await AuthenticationService.GetUserAsync(AuthenticationState))!;
		_breadcrumbs = new List<BreadcrumbItem>
		{
			new BreadcrumbItem("BackOffice", "/back-office"),
			new BreadcrumbItem("Utilisateurs", null, true)
		};
		await RefreshDataAsync();
		StateHasChanged();
	}

	private async Task AddUserAsync()
	{
		var parameters = new DialogParameters<EditUserDialog> { { x => x.Title, "Ajout d'un utilisateur" } };
		var instance = await DialogService.ShowAsync<AddUserDialog>(null, parameters, Hardcoded.Components.DialogOptions);
		var result = await instance.Result;
		if (result is { Data: UserModel model })
		{
			await using var db = await Factory.CreateDbContextAsync();
			db.Users.Add(model);
			db.UsersData.Add(new UserData
			{
				Username = model.Username,
				Pfp = "-1.png",
				ImagePfp = false
			});
			await db.SaveChangesAsync();
			await RefreshDataAsync();
			StateHasChanged();
		}
	}

	private async Task EditUserAsync(UserModel user)
	{
		var parameters = new DialogParameters<EditUserDialog> { { x => x.Title, "Modification d'un utilisateur" }, { x => x.User, user } };
		var instance = await DialogService.ShowAsync<EditUserDialog>(null, parameters, Hardcoded.Components.DialogOptions);
		var result = await instance.Result;
		if (result is { Data: UserModel model })
		{
			await using var db = await Factory.CreateDbContextAsync();
			var old = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => user.Username == x.Username);
			if (old != null)
			{
				old.FirstName = model.FirstName;
				old.LastName = model.LastName;
				old.Role = model.Role;
				old.CreatedAd = model.CreatedAd;
				old.Password = string.IsNullOrWhiteSpace(model.Password) ? old.Password : BC.HashPassword(model.Password);
				
				db.Users.Update(old);
				await db.SaveChangesAsync();
				await RefreshDataAsync();
				StateHasChanged();
			}
		}
	}
	
	private async Task RemoveUserAsync(UserModel model)
	{
		var parameters = new DialogParameters<ConfirmDialog> { { x => x.Title, "Suppression d'un utilisateur" }, { x => x.Content, "Voulez-vous vraiment supprimer cet utilisateur ?\nCette action est irréversible !" } };
		var instance = await DialogService.ShowAsync<ConfirmDialog>(null, parameters, Hardcoded.Components.DialogOptions);
		var result = await instance.Result;
		if (result is { Data: true })
		{
			await using var db = await Factory.CreateDbContextAsync();
			db.Users.Remove(model);
			var oldData = await db.UsersData.FirstOrDefaultAsync(x => x.Username == model.Username);
			if (oldData != null)
			{
				db.UsersData.Remove(oldData);
			}
			
			await db.SaveChangesAsync();
			await RefreshDataAsync();
			StateHasChanged();

			if (model.Username == _user.Username)
			{
				Snackbar.Add("Votre compte a été supprimé. Redirection en cours...", Severity.Warning, options =>
				{
					options.CloseAfterNavigation = false;
					options.ShowTransitionDuration = 250;
				});
				await Task.Delay(1500);
				NavManager.NavigateTo("/api/authentication/logout", true);
			}
		}
	}

	private async Task RefreshDataAsync()
	{
		await using var db = await Factory.CreateDbContextAsync();
		_users = await db.Users.AsNoTracking().ToListAsync();
	}
}