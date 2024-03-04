using Microsoft.AspNetCore.Components;

namespace PokeHama.Components.Pages.UserAccounts;

public partial class ProfileOverview
{
    [Parameter] public string? Username { get; set; }
}