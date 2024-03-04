using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using PokeHama.Databases;
using BC = BCrypt.Net.BCrypt;
using Timer = System.Timers.Timer;

namespace PokeHama.Components.Pages.Authentication;

public partial class Login
{
    [Inject] public IDbContextFactory<UtilityContext> UtilityFactory { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

    private LoginModel _model = new();
    private bool _errorMessage;
    private string _username = string.Empty;
    private string _password = string.Empty;

    private bool _shown;
    public string PasswordIcon => _shown ? Icons.Material.Filled.Visibility : Icons.Material.Filled.VisibilityOff;
    public InputType PasswordInputType => _shown ? InputType.Text : InputType.Password;

    protected override async Task OnInitializedAsync()
    {
        // Checks if the user is already authenticated
        var authenticationState = await AuthenticationStateTask;
        if (authenticationState.User.FindFirst(x => x.Type == ClaimTypes.Surname) != null) NavManager.NavigateTo("/", true);
    }

    private async Task TrySignInAsync()
    {
        var utilityDb = await UtilityFactory.CreateDbContextAsync();
        var users = await utilityDb.Users.ToListAsync();
        var user = users.FirstOrDefault(x => x.Username == _username && BC.Verify(_password, x.Password));

        if (user != null)
        {
            NavManager.NavigateTo($"/api/authenticate/login?username={user.Username}&role={user.Role}", true);
        }
        else
        {
            _model = new();
            _errorMessage = true;
            var timer = new Timer(5000);
            timer.Elapsed += (obj, e) => { _errorMessage = false; _ = InvokeAsync(StateHasChanged); timer.Dispose(); };
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