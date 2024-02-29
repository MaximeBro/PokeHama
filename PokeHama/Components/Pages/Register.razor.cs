using System.ComponentModel.DataAnnotations;
using MudBlazor;

namespace PokeHama.Components.Pages;

public partial class Register
{
	private RegisterModel _model = new();

	private string _firstname = string.Empty;
	private string _lastname = string.Empty;
	private string _username = string.Empty;
	private string _password = string.Empty;
	private string _confirm = string.Empty;

	private bool _passwordShown = false;
	public string PasswordIcon => _passwordShown ? Icons.Material.Filled.Visibility : Icons.Material.Filled.VisibilityOff;
	public InputType PasswordInputType => _passwordShown ? InputType.Text : InputType.Password;
	
	private bool _confirmShown = false;
	public string ConfirmIcon => _confirmShown ? Icons.Material.Filled.Visibility : Icons.Material.Filled.VisibilityOff;
	public InputType ConfirmInputType => _confirmShown ? InputType.Text : InputType.Password;

	private bool _errorMessage;
	private string _errorMessageText = string.Empty;

	private async Task TrySignUpAsync()
	{
		
	}

	private sealed class RegisterModel
	{
		[Required(ErrorMessage = "Veuillez spécifier votre prénom")]
		public string Firstname { get; set; } = string.Empty;
		
		[Required(ErrorMessage = "Veuillez spécifier votre nom")]
		public string Lastname { get; set; } = string.Empty;
		
		[Required(ErrorMessage = "Veuillez spécifier un identifiant")]
		public string Username { get; set; } = string.Empty;
		
		[Required(ErrorMessage = "Veuillez spécifier votre mot de passe")]
		public string Password { get; set; } = string.Empty;
		
		[Required(ErrorMessage = "Veuillez confirmer votre mot de passe")]
		public string Confirm { get; set; } = string.Empty;
	}
}