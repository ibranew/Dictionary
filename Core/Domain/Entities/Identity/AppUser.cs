using Microsoft.AspNetCore.Identity;

namespace Dictionary.Domain.Entities.Identity;

public class AppUser : IdentityUser<int>
{
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}