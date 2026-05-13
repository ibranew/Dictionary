using Dictionary.Domain.Entities;
using Dictionary.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.Application.Common.Interfaces;

/// <summary>
/// Uygulamanın ana veri erişim katmanını temsil eder.
/// Sözlük sistemi için tüm domain entity'lerine erişim sağlar.
/// </summary>
public interface IAppDbContext
{
    /// <summary>Diller (TR, EN, KO vb.)</summary>
    DbSet<Language> Languages { get; }
    /// <summary>Yazı sistemleri (Hangul, Hanja, Latin vb.)</summary>
    DbSet<Script> Scripts { get; }

    /// <summary>Sözlük girişleri (kelime başlıkları)</summary>
    DbSet<Entry> Entries { get; }
    
    /// <summary>Entry Telafuzları</summary>
    DbSet<Pronunciation> Pronunciations { get; }

    /// <summary>Kelime çekimli/türemiş formları</summary>
    DbSet<EntryForm> EntryForms { get; }

    /// <summary>Kelime anlamları (semantic layer)</summary>
    DbSet<Sense> Senses { get; }

    /// <summary>Çeviri ilişkileri (Sense → Sense)</summary>
    DbSet<SenseTranslation> SenseTranslations { get; }

    /// <summary>Çok dilli anlam tanımları</summary>
    DbSet<SenseDefinition> SenseDefinitions { get; }

    /// <summary>Etiketler (argo, tıp, resmi vb.)</summary>
    DbSet<Label> Labels { get; }
    DbSet<LabelType> LabelTypes { get; }

    /// <summary>Sense ↔ Label ilişkileri</summary>
    DbSet<SenseLabel> SenseLabels { get; }

    /// <summary>Örnek cümleler</summary>
    DbSet<Example> Examples { get; }
    /// <summary>Örnek cümle çevirileri</summary>
    DbSet<ExampleTranslation> ExampleTranslations { get; set; }

    /// <summary>Sense değişiklik geçmişi (audit log)</summary>
    DbSet<SenseHistory> SenseHistories { get; }

    /// <summary>Kelime köken bilgileri (Hanja, Latince vb.)</summary>
    DbSet<Etymology> Etymologies { get; }

    /// <summary>Sense ilişkileri (synonym, antonym vb.)</summary>
    DbSet<SenseRelation> SenseRelations { get; }
    DbSet<InflectionType> InflectionTypes { get; }

    /// <summary>
    /// Identity entities
    /// </summary>
    DbSet<RefreshToken> RefreshTokens { get; set; }

    /// <summary>Veritabanına değişiklikleri kaydeder</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    void AddRange(params object[] entities);
}