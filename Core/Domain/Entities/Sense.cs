

using Dictionary.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Dictionary.Domain.Entities;

// ─────────────────────────────────────────────
// SENSE
// ─────────────────────────────────────────────

/// <summary>
/// Bir Entry'nin tek bir anlamını temsil eden atom birim.
/// Tüm çeviri ilişkileri bu seviyede kurulur.
/// Soft delete destekler.
/// </summary>
public class Sense : SoftDeletableEntity
{
    /// <summary>Kelime türü. Örn: "fiil", "isim", "sıfat"</summary>
    public PartOfSpeechType PartOfSpeechType { get; set; } 

    /// <summary>Bu anlamın kısa açıklaması. Entry Hangi dilde ise o dilde yazılır</summary>
    public ICollection<SenseDefinition> Definitions { get; set; } = [];

    /// <summary>Aynı Entry içindeki sıralama. 1 = birincil anlam.</summary>
    public int Order { get; set; }

    /// <summary>
    /// Alt anlam desteği için üst Sense.
    /// Null ise bu bir ana anlamdır.
    /// </summary>
    public int? ParentSenseId { get; set; }
    public Sense? ParentSense { get; set; }
    public ICollection<Sense> ChildSenses { get; set; } = [];

    public int EntryId { get; set; }
    public Entry Entry { get; set; } = null!;

    public ICollection<SenseLabel> SenseLabels { get; set; } = [];
    public ICollection<Example> Examples { get; set; } = [];


    /// <summary>
    /// Sense'e ait etimolojik kökenleri temsil eder.
    /// Örn: Hanja (Çince karakter kökeni), Latince veya diğer kaynak diller.
    /// </summary>
    public ICollection<Etymology> Etymologies { get; set; } = [];

    /// <summary>Bu Sense'den diğer dillerdeki Sense'lere giden çeviriler.</summary>
    public ICollection<SenseTranslation> TranslationsFrom { get; set; } = [];

    /// <summary>Diğer Sense'lerden bu Sense'e gelen çeviriler.</summary>
    public ICollection<SenseTranslation> TranslationsTo { get; set; } = [];

    public ICollection<SenseHistory> SenseHistories { get; set; } = [];

}

// ─────────────────────────────────────────────
// SENSE TRANSLATION
// ─────────────────────────────────────────────

/// <summary>
/// SenseTranslation, iki Sense arasında yönlü (A → B) bir çeviri ilişkisidir.
/// 
/// Bu modelde:
/// - Her çeviri yalnızca tek yönde saklanır.
/// - Reverse (B → A) kayıt otomatik olarak oluşturulmaz.
/// - Çift yönlü görünüm (A ⇄ B) application/query katmanında projection ile sağlanır.
/// 
/// Tasarım amacı:
/// - Veri tekrarını önlemek
/// - Çeviri yönünü ve bağlamını korumak
/// - Graph yapısında kontrolü application layer'a bırakmak
/// 
/// Ek bilgiler:
/// - Confidence: Anlamsal eşdeğerlik skoru (1: zayıf, 5: tam eşdeğer)
/// - Note: Çeviri bağlamı veya kullanım notu
/// </summary>
public class SenseTranslation : BaseEntity
{
    public int SourceSenseId { get; set; }
    public Sense SourceSense { get; set; } = null!;

    public int TargetSenseId { get; set; }
    public Sense TargetSense { get; set; } = null!;

    /// <summary>
    /// Anlamsal eşdeğerlik skoru. 1 = zayıf, 5 = tam eşdeğer.
    /// </summary>
    [Range(1, 5)]
    public int Confidence { get; set; } = 3;

    /// <summary>Çeviri için ek açıklama. Default Türkçe seçilebilir Örn: "ilaç bağlamında kullanılır"</summary>
    [MaxLength(300)]
    public string? Note { get; set; }
}

/// <summary>
///  SenseDefinition Çok dilli Destek
/// </summary>
public class SenseDefinition : BaseEntity
{
    public int SenseId { get; set; }
    public Sense Sense { get; set; } = null!;

    public int LanguageId { get; set; }
    public Language Language { get; set; } = null!;

    [MaxLength(500)]
    public string Text { get; set; } = null!;
}

/// <summary>
/// İki anlam arasındaki ilişki
/// </summary>
public class SenseRelation : BaseEntity
{
    public int SourceSenseId { get; set; }
    public Sense SourceSense { get; set; } = null!;

    public int TargetSenseId { get; set; }
    public Sense TargetSense { get; set; } = null!;

    /// <summary>
    /// ilişki türü
    /// synonym, antonym, related, hypernym vs.
    /// </summary>
    public SenseRelationType RelationType { get; set; } 

    /// <summary>
    /// Güç seviyesi (opsiyonel ama çok iyi olur)
    /// </summary>
    public int? Strength { get; set; } // 1-5
}


/// <summary>
/// Bir Sense'in etimolojik köken bilgisini temsil eder.
/// Kelimenin hangi dilden, hangi formdan türediğini açıklar.
/// Örn: Korece "학교" kelimesinin Çince (Hanja) kökeni.
/// </summary>
public class Etymology : BaseEntity
{
    /// <summary>
    /// Bu etimolojinin bağlı olduğu Sense.
    /// </summary>
    public int SenseId { get; set; }
    public Sense Sense { get; set; } = null!;

    /// <summary>
    /// Köken dil.
    /// Örn: zh (Çince), la (Latince), grc (Antik Yunanca)
    /// </summary>
    public int SourceLanguageId { get; set; }
    public Language SourceLanguage { get; set; } = null!;

    /// <summary>Kökenin ait olduğu karakter sistemi.</summary>
    public int? ScriptId { get; set; }
    public Script? Script { get; set; }

    /// <summary>
    /// Orijinal köken formu / karakter.
    /// Örn: 學校
    /// </summary>
    [Required, MaxLength(200)]
    public string Word { get; set; } = null!;

    /// <summary>
    /// Anlam açıklaması veya açıklayıcı not.
    /// </summary>
    [MaxLength(500)]
    public string? Meaning { get; set; }

    /// <summary>
    /// Etimolojik zincirdeki sıralama.
    /// Örn: 1 = ana köken, 2 = türev parça
    /// </summary>
    public int Order { get; set; }
}

/// <summary>
/// İki <see cref="Sense"/> arasındaki anlamsal ilişki türünü belirtir.
/// </summary>
public enum SenseRelationType
{
    /// <summary>
    /// Aynı veya çok benzer anlamı paylaşan kavramlar.
    /// Örn: "big" ↔ "large"
    /// </summary>
    Synonym,

    /// <summary>
    /// Zıt anlamlı kavramlar.
    /// Örn: "big" ↔ "small"
    /// </summary>
    Antonym,

    /// <summary>
    /// Doğrudan eş anlamlı veya zıt olmayan, ancak anlamsal olarak ilişkili kavramlar.
    /// Örn: "doctor" ↔ "hospital"
    /// </summary>
    Related,

    /// <summary>
    /// Daha genel (üst) kavramı ifade eder.
    /// Örn: "animal" → "dog"
    /// (animal, dog'un hypernym'idir)
    /// </summary>
    Hypernym,

    /// <summary>
    /// Daha özel (alt) kavramı ifade eder.
    /// Örn: "dog" → "animal"
    /// (dog, animal'ın hyponym'idir)
    /// </summary>
    Hyponym
}

/// <summary>
/// Bir kelimenin dilbilgisel türünü (Part of Speech - POS) belirtir.
/// Bu enum, bir <see cref="Sense"/> nesnesinin hangi gramer kategorisine ait olduğunu tanımlar.
/// </summary>
public enum PartOfSpeechType
{
    /// <summary>
    /// İsim. Kişi, yer, nesne veya kavramları ifade eder.
    /// Örn: "kitap", "masa", "insan"
    /// </summary>
    Noun = 1,

    /// <summary>
    /// Fiil. Eylem veya oluş bildirir.
    /// Örn: "yemek", "gitmek", "koşmak"
    /// </summary>
    Verb = 2,

    /// <summary>
    /// Sıfat. İsimleri niteler veya belirtir.
    /// Örn: "büyük", "hızlı", "güzel"
    /// </summary>
    Adjective = 3,

    /// <summary>
    /// Zarf. Fiilleri, sıfatları veya diğer zarfları niteler.
    /// Örn: "hızlıca", "çok", "hemen"
    /// </summary>
    Adverb = 4,

    /// <summary>
    /// Zamir. İsimlerin yerine kullanılan kelimeler.
    /// Örn: "ben", "sen", "o", "biz"
    /// </summary>
    Pronoun = 5,

    /// <summary>
    /// Edat (ilgeç). Cümlede diğer kelimelerle anlam ilişkisi kurar.
    /// Örn: "ile", "için", "gibi"
    /// </summary>
    Preposition = 6,

    /// <summary>
    /// Bağlaç. Kelime veya cümleleri birbirine bağlar.
    /// Örn: "ve", "ama", "çünkü"
    /// </summary>
    Conjunction = 7,

    /// <summary>
    /// Ünlem. Duygu veya ani tepkileri ifade eder.
    /// Örn: "hey!", "ah!", "of!"
    /// </summary>
    Interjection = 8
}