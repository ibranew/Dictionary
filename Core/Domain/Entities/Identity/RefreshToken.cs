namespace Dictionary.Domain.Entities.Identity;
public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }

    // Rotation: eski token iptal edilince yerine geçen token
    public string? ReplacedByToken { get; set; }

    // Hangi IP'den oluşturuldu
    public string? CreatedByIp { get; set; }
    public string? RevokedByIp { get; set; }

    // Computed
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;

    // FK
    public int UserId { get; set; }
    public AppUser User { get; set; } = null!;
}