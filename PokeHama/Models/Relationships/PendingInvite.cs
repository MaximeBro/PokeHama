using System.ComponentModel.DataAnnotations;

namespace PokeHama.Models.Relationships;

public class PendingInvite
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(50)] 
    public string From { get; set; } = null!;
    [MaxLength(50)]
    public string To { get; set; } = null!;
}