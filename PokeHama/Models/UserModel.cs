namespace PokeHama.Models;

public class UserModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public UserRole Role { get; set; }
    public DateTime CreatedAd { get; set; }
}