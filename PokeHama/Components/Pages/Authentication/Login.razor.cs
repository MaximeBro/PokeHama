using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using PokeHama.Services;
using Timer = System.Timers.Timer;

namespace PokeHama.Components.Pages.Authentication;

public partial class Login
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;
    [Inject] public AuthenticationService AuthenticationService { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;

    private LoginModel _model = new();
    private bool _errorMessage;

    private bool _shown;
    private string PasswordIcon => _shown ? Icons.Material.Filled.Visibility : Icons.Material.Filled.VisibilityOff;
    private InputType PasswordInputType => _shown ? InputType.Text : InputType.Password;

    protected override async Task OnInitializedAsync()
    {
        // Checks if the user is already authenticated
        var authenticationState = await AuthenticationStateTask;
        if (authenticationState.User.FindFirst(x => x.Type == ClaimTypes.Name) != null) NavManager.NavigateTo("/", true);
    }

    private async Task OnSubmitAsync()
    {
        var authenticated = await AuthenticationService.ValidateLoginAsync(_model.Username, _model.Password);
        if (!authenticated)
        {
            _model = new();
            _errorMessage = true;
            var timer = new Timer(3500);
            timer.Elapsed += (_, _) => { _errorMessage = false; _ = InvokeAsync(StateHasChanged); timer.Dispose(); };
            timer.Start();
        }
    }

    private sealed class LoginModel
    {
        [Required(ErrorMessage = "Veuillez spécifier votre identifiant.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Veuillez spécifier votre mot de passe.")]
        public string Password { get; set; } = string.Empty;
    } 
}