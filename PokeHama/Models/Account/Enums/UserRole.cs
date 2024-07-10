using System.ComponentModel;

namespace PokeHama.Models.Account.Enums;

public enum UserRole
{
    [Description("Visiteur")]
    Guest,
    
    [Description("Utilisateur")]
    User,
    
    [Description("Administrateur")]
    Admin
}