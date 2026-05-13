using Dictionary.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Dictionary.Domain.Entities;

// ─────────────────────────────────────────────
// AUDIT / HISTORY
// ─────────────────────────────────────────────

/// <summary>
/// Bir Sense üzerinde yapılan değişikliklerin tarihçesini tutar.
/// Editör ekibinde kim ne değiştirdi sorusunu yanıtlar.
/// </summary>
public class SenseHistory : BaseEntity
{
    public int SenseId { get; set; }
    public Sense Sense { get; set; } = null!;

    /// <summary>Değişikliği yapan kullanıcının adı ya da kimliği.</summary>
    [Required, MaxLength(200)]
    public string ChangedBy { get; set; } = null!;

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Değişiklik öncesi Sense verisinin JSON anlık görüntüsü.</summary>
    public string OldValueJson { get; set; } = null!;
}