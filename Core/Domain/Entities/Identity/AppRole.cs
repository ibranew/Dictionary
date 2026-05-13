using Microsoft.AspNetCore.Identity;
namespace Dictionary.Domain.Entities.Identity;
public class AppRole : IdentityRole<int>
{
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}