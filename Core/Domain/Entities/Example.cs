

using Dictionary.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Dictionary.Domain.Entities;
// ─────────────────────────────────────────────
// EXAMPLE
// ─────────────────────────────────────────────

/// <summary>
/// Bir Sense'e ait örnek cümle ve çevirileri
/// </summary>
public class Example : BaseEntity
{
    public int SenseId { get; set; }
    public Sense Sense { get; set; } = null!;

    public string SourceText { get; set; } = null!;

    public ICollection<ExampleTranslation> Translations { get; set; } = [];
}

/// <summary>
/// Bir Sense'e ait örnek cümle çevirilerini 
/// </summary>
public class ExampleTranslation : BaseEntity
{
    public int ExampleId { get; set; }
    public Example Example { get; set; } = null!;
    public int LanguageId { get; set; }
    public Language Language { get; set; } = null!;
    public string Text { get; set; } = null!;
}