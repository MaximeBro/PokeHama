using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using PokeHama.Databases;
using PokeHama.Extensions;
using PokeHama.Models;
using AuthenticationService = PokeHama.Services.AuthenticationService;
using BC = BCrypt.Net.BCrypt;
using Timer = System.Timers.Timer;

namespace PokeHama.Components.Pages;

public partial class Login
{
    [Inject] public IDbContextFactory<UtilityContext> UtilityFactory { get; set; } = null!;
    [Inject] public AuthenticationService AuthenticationService { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;

    private LoginModel _model = new();
    private bool _errorMessage;
    private string _username = string.Empty;
    private string _password = string.Empty;

    private bool _shown;
    public string PasswordIcon => _shown ? Icons.Material.Filled.Visibility : Icons.Material.Filled.VisibilityOff;
    public InputType PasswordInputType => _shown ? InputType.Text : InputType.Password;

    protected override async Task OnInitializedAsync()
    {
        var isAuthenticated = await AuthenticationService.IsAuthenticatedAsync();
        if (isAuthenticated) NavManager.NavigateTo("/", true);
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