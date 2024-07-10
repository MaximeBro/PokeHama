using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using PokeHama.Databases;
using PokeHama.Models.Account;

namespace PokeHama.Components.Pages.BackOffice;

public partial class UsersManagement
{
	[Inject] public IDbContextFactory<UtilityContext> Factory { get; set; } = null!;

	private List<BreadcrumbItem> _breadcrumbs = [];
	private List<UserModel> _users = [];

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
		_breadcrumbs = new List<BreadcrumbItem>
		{
			new BreadcrumbItem("BackOffice", "/back-office"),
			new BreadcrumbItem("Utilisateurs", null, true)
		};
		await RefreshDataAsync();
		StateHasChanged();
	}

	private async Task RefreshDataAsync()
	{
		await using var db = await Factory.CreateDbContextAsync();
		_users = await db.Users.AsNoTracking().ToListAsync();
	}
}