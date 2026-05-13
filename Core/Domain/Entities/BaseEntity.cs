using System.ComponentModel.DataAnnotations;

namespace Dictionary.Domain.Entities;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
}
// ─────────────────────────────────────────────
// BASE
// ─────────────────────────────────────────────
/// <summary>
/// Tüm entity'lerin türediği taban sınıf.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
}

/// <summary>
/// Soft delete destekleyen temel entity sınıfı.
/// Silme işlemi veritabanından kaldırmak yerine işaretleme (IsDeleted) yapar.
/// Ayrıca geri alma (undo) senaryoları için restore zamanı tutulur.
/// </summary>
public abstract class SoftDeletableEntity : BaseEntity, ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

// ─────────────────────────────────────────────
// LANGUAGE
// ─────────────────────────────────────────────
/// <summary>
/// Sistemdeki dilleri temsil eder. Örn: Korece (ko), Türkçe (tr).
/// </summary>
public class Language : BaseEntity
{
    /// <summary>ISO 639-1 dil kodu. Örn: "ko", "tr"</summary>
    [Required, MaxLength(10)]
    public string Code { get; set; } = null!;
    /// <summary>Dilin tam adı. Örn: "Korece", "Türkçe"</summary>
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;
    public ICollection<Entry> Entries { get; set; } = [];
}


/// <summary>
/// Bir dilin yazı sistemini (script) temsil eder.
/// Örn: Hangul (Kore alfabesi), Hanja (Çin karakter sistemi), Latin.
/// </summary>
public class Script : BaseEntity
{
    /// <summary>
    /// Yazı sisteminin kısa kodu.
    /// Örn: "hangul", "hanja", "latin"
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Yazı sisteminin görünen adı.
    /// Örn: "Hangul", "Hanja", "Latin"
    /// </summary>
    public string Name { get; set; } = null!;
}
