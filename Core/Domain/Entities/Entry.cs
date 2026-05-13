using System.ComponentModel.DataAnnotations;
namespace Dictionary.Domain.Entities;

// ─────────────────────────────────────────────
// ENTRY
// ─────────────────────────────────────────────

/// <summary>
/// Sözlükteki bir kelimenin ham yazım formunu temsil eder.
/// Birden fazla anlam (Sense) barındırabilir.
/// Soft delete destekler.
/// </summary>
public class Entry : SoftDeletableEntity
{
    /// <summary>Kelimenin sözlük formu. Örn: "먹다", "yemek"</summary>
    [Required, MaxLength(200)]
    public string Headword { get; set; } = null!;
    [Required, MaxLength(200)]
    public string HeadwordNormalized { get; set; } = null!;

    /// <summary>Telaffuzlar</summary>
    public ICollection<Pronunciation> Pronunciations { get; set; } = [];

    /// <summary>Kelimenin ait olduğu dil.</summary>
    public int LanguageId { get; set; }
    public Language Language { get; set; } = null!;

    /// <summary>Kelimenin ait olduğu karakter sistemi.</summary>
    public int? ScriptId { get; set; }
    public Script? Script { get; set; }

    /// <summary>Kelimenin çekimli/türemiş formları. Örn: 먹었어요, yedi</summary>
    public ICollection<EntryForm> Forms { get; set; } = [];

    /// <summary>Kelimenin tüm anlamları.</summary>
    public ICollection<Sense> Senses { get; set; } = [];
   
}



/// <summary>
/// Bir kelimenin (Entry) telaffuz bilgisini temsil eder.
/// 
/// - IPA: Uluslararası fonetik alfabe ile bilimsel telaffuz.
/// - Romanization: Latin harfleriyle okunuş (örn: meokda).
/// - Native: Yerel dildeki yaklaşık okunuş (örn: 먹따).
///
/// Amaç:
/// - Farklı kullanıcı tiplerine (öğrenci, akademik) uygun veri sunabilmek.
///
/// Not:
/// - Romanization genellikle zorunlu tutulabilir (önerilir).
/// - IPA opsiyonel bırakılabilir.
/// </summary>
public class Pronunciation : BaseEntity
{
    /// <summary>
    /// Telaffuzun ait olduğu Entry (kelime).
    /// </summary>
    public int EntryId { get; set; }
    public Entry Entry { get; set; } = null!;

    /// <summary>
    /// Telaffuz metni.
    /// Örn: "meokda", "먹따", "mʌk̚.t͈a"
    /// </summary>
    public string Text { get; set; } = null!;

    /// <summary>
    /// Telaffuz türü.
    /// </summary>
    public PronunciationType Type { get; set; }

    /// <summary>
    /// (Opsiyonel) Telaffuzun ait olduğu dil.
    /// Özellikle çok dilli telaffuzlarda kullanılabilir.
    /// </summary>
    public int? LanguageId { get; set; }
}
/// <summary>
/// Telaffuzun hangi formatta yazıldığını belirtir.
/// </summary>
public enum PronunciationType
{
    /// <summary>
    /// Uluslararası Fonetik Alfabe (IPA).
    /// </summary>
    IPA,

    /// <summary>
    /// Latin harfleri ile yazılmış okunuş.
    /// </summary>
    Romanization,

    /// <summary>
    /// Yerel dildeki yaklaşık okunuş.
    /// Örn: Korece için "먹따"
    /// </summary>
    Native
}



// ─────────────────────────────────────────────
// ENTRY FORM  (morfoloji)
// ─────────────────────────────────────────────

/// <summary>
/// Bir Entry'nin çekimli ya da türemiş yazım formlarını tutar.
/// Kore dili gibi eklemeli diller için arama kalitesini artırır.
/// </summary>
public class EntryForm : BaseEntity
{
    /// <summary>Çekimli form. Örn: "먹었어요", "먹고"</summary>
    [Required, MaxLength(200)]
    public string Form { get; set; } = null!;
    [Required, MaxLength(200)]
    public string FormNormalized { get; set; } = null!;

    /// <summary>Çekim türü. Örn: "past", "gerund", "honorific"</summary>
    public int? InflectionTypeId { get; set; }
    public InflectionType? InflectionType { get; set; }

    public int EntryId { get; set; }
    public Entry Entry { get; set; } = null!;
}

/// <summary>
/// Bir dildeki çekim (inflection) türlerini temsil eder.
/// Entry'lerin farklı dilbilgisel formlarını kategorize etmek için kullanılır.
/// Örn: İngilizce için "past", "gerund"; Korece için "honorific", "polite".
/// </summary>
public class InflectionType : BaseEntity
{
    /// <summary>
    /// Çekim türünün adı.
    /// Örn: "past", "gerund", "honorific".
    /// </summary>
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    public int LanguageId { get; set; }

    /// <summary>
    /// Çekim türünün ait olduğu dil.
    /// </summary>
    public Language Language { get; set; } = null!;
}
