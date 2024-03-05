using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using PokeHama.Databases;
using PokeHama.Models;
using PokeHama.Models.Account;
using PokeHama.Models.Account.Enums;
using Timer = System.Timers.Timer;
using BC = BCrypt.Net.BCrypt;

namespace PokeHama.Components.Pages.Authentication;

public partial class Register
{
	[Inject] public IDbContextFactory<UtilityContext> UtilityFactory { get; set; } = null!;
	[Inject] public NavigationManager NavManager { get; set; } = null!;

	private MudForm _form = null!;
	private RegisterModel _model = new();
	private bool _isValid;

	private bool _passwordShown = false;
	public string PasswordIcon => _passwordShown ? Icons.Material.Filled.Visibility : Icons.Material.Filled.VisibilityOff;
	public InputType PasswordInputType => _passwordShown ? InputType.Text : InputType.Password;
	
	private bool _confirmShown = false;
	public string ConfirmIcon => _confirmShown ? Icons.Material.Filled.Visibility : Icons.Material.Filled.VisibilityOff;
	public InputType ConfirmInputType => _confirmShown ? InputType.Text : InputType.Password;
	
	private bool _errorMessage;
	private string _errorMessageText = string.Empty;
	private bool _loading;

	private async Task TrySignUpAsync()
	{
		await _form.Validate();
		
		if (_model.Password != _model.Confirm)
		{
			_errorMessage = true;
			_errorMessageText = "Les mots de passe ne concordent pas !";
			StartErrorTimer();
			return;
		}

		var utilityDb = await UtilityFactory.CreateDbContextAsync();
		var potentialUser = utilityDb.Users.FirstOrDefault(x => x.Username == _model.Username); 
		if (potentialUser != null)
		{
			_errorMessage = true;
			_errorMessageText = "Un utilisateur avec cet identifiant existe déjà !";
			StartErrorTimer();
		}
		else if(_isValid)
		{
			var user = new UserModel
			{
				FirstName = _model.Firstname,
				LastName = _model.Lastname,
				Username = _model.Username,
				Password = BC.HashPassword(_model.Password),
				CreatedAd = DateTime.UtcNow,
				Role = UserRole.User
			};

			_loading = true;
			utilityDb.Users.Add(user);
			utilityDb.UsersData.Add(new UserData
			{
				Username = user.Username,
				Pfp = "-1.png",
				ImagePfp = false
			});
			await utilityDb.SaveChangesAsync();
			await utilityDb.DisposeAsync();
			_loading = false;
			NavManager.NavigateTo($"/api/authenticate/login?username={user.Username}&role={user.Role}", true);
		}
	}

	private void StartErrorTimer()
	{
		var timer = new Timer(5000);
		timer.Elapsed += (obj, e) => { _errorMessage = false; _ = InvokeAsync(StateHasChanged); timer.Dispose(); };
		timer.Start();
	}

	private sealed class RegisterModel
	{
		public string Firstname { get; set; } = string.Empty;
		
		public string Lastname { get; set; } = string.Empty;
		
		public string Username { get; set; } = string.Empty;
		
		public string Password { get; set; } = string.Empty;
		
		public string Confirm { get; set; } = string.Empty;
		
		public bool PrivacyPolicy { get; set; }
	}
}