

using Dictionary.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Dictionary.Domain.Entities;

// ─────────────────────────────────────────────
// LABEL
// ─────────────────────────────────────────────

/// <summary>
/// Bir Sense'e atanabilen bağlam etiketi.
/// Kullanım alanı, dil kaydı veya bölgesel bilgi içerebilir.
/// </summary>
public class Label : BaseEntity
{
    /// <summary>Etiket adı. Örn: "argo", "resmi", "tıp", "mutfak"</summary>
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Etiket kategorisi.
    /// Örn: "register" (argo/resmi), "domain" (alan), "region" (lehçe)
    /// </summary>
    public int LabelTypeId { get; set; }
    public LabelType LabelType { get; set; } = null!;

    public ICollection<SenseLabel> SenseLabels { get; set; } = [];
}

public class LabelType : BaseEntity
{
    public string Code { get; set; } = null!; // register, domain
    public string DisplayName { get; set; } = null!; // "Kayıt türü"
    public ICollection<Label> Labels { get; set; } = [];
}

// ─────────────────────────────────────────────
// SENSE LABEL  (many-to-many köprü)
// ─────────────────────────────────────────────

/// <summary>
/// Sense ile Label arasındaki many-to-many köprü tablosu.
/// </summary>
public class SenseLabel
{
    public int SenseId { get; set; }
    public Sense Sense { get; set; } = null!;

    public int LabelId { get; set; }
    public Label Label { get; set; } = null!;
}
