using Dictionary.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dictionary.Persistence.EntityConfigurations;

/// <summary>
/// Language tablosu konfigürasyonu.
/// Code alanı unique index ile korunur.
/// </summary>
public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("Languages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Aynı dil kodu iki kez eklenemesin
        builder.HasIndex(x => x.Code)
            .IsUnique();

        //Production Seviyesinde zaten lang silinmeyecek
    }
}
// ───────────────────────────────────────────────────────────

/// <summary>
/// Script tablosu konfigürasyonu.
/// Yazı sistemlerini (Hangul, Hanja, Latin vb.) yönetir.
/// </summary>
public class ScriptConfiguration : IEntityTypeConfiguration<Script>
{
    public void Configure(EntityTypeBuilder<Script> builder)
    {
        builder.ToTable("Scripts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Aynı kod iki kez eklenemesin
        builder.HasIndex(x => x.Code)
            .IsUnique();
    }
}
// ───────────────────────────────────────────────────────────

/// <summary>
/// Entry tablosu konfigürasyonu.
/// Headword + LanguageId birlikte unique: aynı dilde aynı kelime iki kez girilemesin.
/// </summary>
public class EntryConfiguration : IEntityTypeConfiguration<Entry>
{
    public void Configure(EntityTypeBuilder<Entry> builder)
    {
        builder.ToTable("Entries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Headword)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.HeadwordNormalized)
           .IsRequired()
           .HasMaxLength(200);


        // Dil ilişkisi — dil silinirse entry'ler korunur (Restrict)
        builder.HasOne(x => x.Language)
            .WithMany(x => x.Entries)
            .HasForeignKey(x => x.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Script)
         .WithMany()
         .HasForeignKey(x => x.ScriptId)
         .OnDelete(DeleteBehavior.Restrict)
         .IsRequired(false);

        builder.HasIndex(x => new { x.HeadwordNormalized, x.LanguageId })
          .IsUnique();

        // Entry arama
        builder.HasIndex(x => x.HeadwordNormalized);
    }
}

// ───────────────────────────────────────────────────────────

/// <summary>
/// EntryForm tablosu konfigürasyonu.
/// Çekimli form araması için Form alanına index eklenir.
/// </summary>
public class EntryFormConfiguration : IEntityTypeConfiguration<EntryForm>
{
    public void Configure(EntityTypeBuilder<EntryForm> builder)
    {
        builder.ToTable("EntryForms");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Form)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.InflectionTypeId);

        builder.HasOne(x => x.Entry)
            .WithMany(x => x.Forms)
            .HasForeignKey(x => x.EntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.FormNormalized)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(x => new { x.FormNormalized, x.EntryId }).IsUnique();
    }
}

// ───────────────────────────────────────────────────────────

/// <summary>
/// Pronunciation tablosu konfigürasyonu.
/// Bir Entry'ye ait farklı telaffuz türlerini (IPA, Romanization, Native) tutar.
/// 
/// Kurallar:
/// - Her Entry için her Type sadece bir kez olabilir (unique constraint).
/// - Entry silinirse telaffuzlar da silinir (Cascade).
/// </summary>
public class PronunciationConfiguration : IEntityTypeConfiguration<Pronunciation>
{
    public void Configure(EntityTypeBuilder<Pronunciation> builder)
    {
        builder.ToTable("Pronunciations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Text)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Type)
            .IsRequired();

        // Entry ilişkisi
        builder.HasOne(x => x.Entry)
            .WithMany(x => x.Pronunciations)
            .HasForeignKey(x => x.EntryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Aynı Entry + Type tekrar edemesin
        builder.HasIndex(x => new { x.EntryId, x.Type })
            .IsUnique();

        // Arama / filtreleme için
        builder.HasIndex(x => x.Type);
    }
}

// ───────────────────────────────────────────────────────────
/// <summary>
/// Sense tablosu konfigürasyonu.
/// Self-referencing hiyerarşi ve çeviri ilişkileri burada kurulur.
/// </summary>
public class SenseConfiguration : IEntityTypeConfiguration<Sense>
{
    public void Configure(EntityTypeBuilder<Sense> builder)
    {
        builder.ToTable("Senses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PartOfSpeechType)
           .IsRequired();

        builder.HasIndex(x => new { x.EntryId, x.Order })
        .IsUnique();

        // Entry ilişkisi
        builder.HasOne(x => x.Entry)
            .WithMany(x => x.Senses)
            .HasForeignKey(x => x.EntryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Self-referencing: alt anlam hiyerarşisi
        builder.HasOne(x => x.ParentSense)
            .WithMany(x => x.ChildSenses)
            .HasForeignKey(x => x.ParentSenseId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // TranslationsFrom: bu Sense kaynak
        builder.HasMany(x => x.TranslationsFrom)
            .WithOne(x => x.SourceSense)
            .HasForeignKey(x => x.SourceSenseId)
            .OnDelete(DeleteBehavior.NoAction);

        // TranslationsTo: bu Sense hedef — Cascade çakışmasını önlemek için NoAction
        builder.HasMany(x => x.TranslationsTo)
            .WithOne(x => x.TargetSense)
            .HasForeignKey(x => x.TargetSenseId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Definitions)
          .WithOne(x => x.Sense)
          .HasForeignKey(x => x.SenseId)
          .OnDelete(DeleteBehavior.Cascade);
    }
}

// ───────────────────────────────────────────────────────────

/// <summary>
/// SenseTranslation tablosu konfigürasyonu.
/// Aynı kaynak-hedef çifti iki kez eklenemesin.
/// </summary>
public class SenseTranslationConfiguration : IEntityTypeConfiguration<SenseTranslation>
{
    public void Configure(EntityTypeBuilder<SenseTranslation> builder)
    {
        ///Not: Bu SQL Server'da çalışır, ama PostgreSQL/MySQL kullanırsan syntax farklıdır.
        builder.ToTable("SenseTranslations", t =>
        {
            t.HasCheckConstraint(
                "CK_SenseTranslation_NoSelf",
                "[SourceSenseId] <> [TargetSenseId]"
            );
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Confidence)
            .IsRequired()
            .HasDefaultValue(3);

        builder.Property(x => x.Note)
            .HasMaxLength(300);

        // Aynı sense çifti tekrar eklenemesin
        builder.HasIndex(x => new { x.SourceSenseId, x.TargetSenseId })
            .IsUnique();

        builder.HasIndex(x => x.TargetSenseId);

    }
}

// ───────────────────────────────────────────────────────────

/// <summary>
/// Label tablosu konfigürasyonu.
/// Aynı isimde iki etiket olamaz.
/// </summary>
public class LabelConfiguration : IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        builder.ToTable("Labels");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        // LabelType ilişkisi
        builder.HasOne(x => x.LabelType)
            .WithMany()
            .HasForeignKey(x => x.LabelTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Aynı isim aynı type içinde tekrar etmesin (daha doğru unique)
        builder.HasIndex(x => new { x.Name, x.LabelTypeId })
            .IsUnique();

        // query performansı
        builder.HasIndex(x => x.LabelTypeId);
    }
}

// ───────────────────────────────────────────────────────────
public class LabelTypeConfiguration : IEntityTypeConfiguration<LabelType>
{
    public void Configure(EntityTypeBuilder<LabelType> builder)
    {
        builder.ToTable("LabelTypes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Code)
            .IsUnique();
    }
}

// ───────────────────────────────────────────────────────────

/// <summary>
/// SenseLabel köprü tablosu konfigürasyonu.
/// Composite primary key ile many-to-many ilişki kurulur.
/// </summary>
public class SenseLabelConfiguration : IEntityTypeConfiguration<SenseLabel>
{
    public void Configure(EntityTypeBuilder<SenseLabel> builder)
    {
        builder.ToTable("SenseLabels");

        // Composite PK — ayrı Id sütunu gereksiz
        builder.HasKey(x => new { x.SenseId, x.LabelId });

        builder.HasOne(x => x.Sense)
            .WithMany(x => x.SenseLabels)
            .HasForeignKey(x => x.SenseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Label)
            .WithMany(x => x.SenseLabels)
            .HasForeignKey(x => x.LabelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

// ───────────────────────────────────────────────────────────

/// <summary>
/// Example tablosu konfigürasyonu.
/// </summary>
public class ExampleConfiguration : IEntityTypeConfiguration<Example>
{
    public void Configure(EntityTypeBuilder<Example> builder)
    {
        builder.ToTable("Examples");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SourceText)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasOne(x => x.Sense)
            .WithMany(x => x.Examples)
            .HasForeignKey(x => x.SenseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.SenseId);

        builder.HasIndex(x => new { x.SenseId, x.SourceText })
          .IsUnique();
    }
}

// ──────────────────────────────────────────────────────────

/// <summary>
/// ExampleTranslation tablosu konfigürasyonu.
/// </summary>
public class ExampleTranslationConfiguration : IEntityTypeConfiguration<ExampleTranslation>
{
    public void Configure(EntityTypeBuilder<ExampleTranslation> builder)
    {
        builder.ToTable("ExampleTranslations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Text)
            .IsRequired()
            .HasMaxLength(1000);

        // ✅ Cascade: Example silinince çevirisi de silinsin
        builder.HasOne(x => x.Example)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.ExampleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Language)
            .WithMany()
            .HasForeignKey(x => x.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasIndex(x => new { x.ExampleId, x.LanguageId })
            .IsUnique();

        builder.HasIndex(x => x.LanguageId);
    }
}

// ───────────────────────────────────────────────────────────

/// <summary>
/// SenseHistory tablosu konfigürasyonu.
/// OldValueJson büyük veri içerebileceği için sınırsız bırakılır.
/// </summary>
public class SenseHistoryConfiguration : IEntityTypeConfiguration<SenseHistory>
{
    public void Configure(EntityTypeBuilder<SenseHistory> builder)
    {
        builder.ToTable("SenseHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ChangedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ChangedAt)
            .IsRequired();

        // JSON snapshot için uzunluk kısıtlaması yok
        // Sql Server Bağımlılı var
        builder.Property(x => x.OldValueJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.HasOne(x => x.Sense)
            .WithMany(x => x.SenseHistories)
            .HasForeignKey(x => x.SenseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.SenseId);
        builder.HasIndex(x => x.ChangedAt);
    }
}

// ───────────────────────────────────────────────────────────

/// <summary>
/// Sense → çok dilli tanım (definition) tablosunun veritabanı konfigürasyonunu tanımlar.
/// Her Sense için farklı dillerde açıklama tutulmasını sağlar.
/// </summary>
public class SenseDefinitionConfiguration : IEntityTypeConfiguration<SenseDefinition>
{
    public void Configure(EntityTypeBuilder<SenseDefinition> builder)
    {
        builder.ToTable("SenseDefinitions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Text)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.LanguageId)
            .IsRequired();
          
        builder.HasOne(x => x.Sense)
            .WithMany(x => x.Definitions)
            .HasForeignKey(x => x.SenseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Language)
            .WithMany()
            .HasForeignKey(x => x.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);



        builder.HasIndex(x => new { x.SenseId, x.LanguageId })
               .IsUnique();
    }
}

// ───────────────────────────────────────────────────────────

/// <summary>
/// İki Sense arasındaki anlamsal ilişkilerin (synonym, antonym, hypernym vb.)
/// veritabanı konfigürasyonunu tanımlar.
/// </summary>
public class SenseRelationConfiguration : IEntityTypeConfiguration<SenseRelation>
{
    public void Configure(EntityTypeBuilder<SenseRelation> builder)
    {
        builder.ToTable("SenseRelations", t =>
        {
            t.HasCheckConstraint(
                "CK_SenseRelation_NoSelf",
                "[SourceSenseId] <> [TargetSenseId]"
            );
        });
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.RelationType);

        builder.HasOne(x => x.SourceSense)
            .WithMany()
            .HasForeignKey(x => x.SourceSenseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.TargetSense)
            .WithMany()
            .HasForeignKey(x => x.TargetSenseId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.SourceSenseId, x.TargetSenseId, x.RelationType })
            .IsUnique();

        builder.HasIndex(x => x.TargetSenseId);


    }
}

// ───────────────────────────────────────────────────────────

/// <summary>
/// Sense'lerin etimolojik köken bilgilerini (Hanja, Latince, Çince vb.)
/// veritabanı konfigürasyonunu tanımlar.
/// </summary>
public class EtymologyConfiguration : IEntityTypeConfiguration<Etymology>
{
    public void Configure(EntityTypeBuilder<Etymology> builder)
    {
        builder.ToTable("Etymologies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Word)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Meaning)
            .HasMaxLength(500);

        builder.HasOne(x => x.Sense)
            .WithMany(x => x.Etymologies)
            .HasForeignKey(x => x.SenseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Script)
           .WithMany()
           .HasForeignKey(x => x.ScriptId)
           .OnDelete(DeleteBehavior.Restrict)
           .IsRequired(false);

        builder.HasOne(x => x.SourceLanguage)
               .WithMany()
               .HasForeignKey(x => x.SourceLanguageId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.SenseId);
        builder.HasIndex(x => x.SourceLanguageId);
    }
}


/// <summary>
/// InflectionType tablosu konfigürasyonu.
/// Dil bazlı çekim türlerini (past, gerund, honorific vs.) yönetir.
/// </summary>
public class InflectionTypeConfiguration : IEntityTypeConfiguration<InflectionType>
{
    public void Configure(EntityTypeBuilder<InflectionType> builder)
    {
        builder.ToTable("InflectionTypes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Dil ilişkisi
        builder.HasOne(x => x.Language)
            .WithMany()
            .HasForeignKey(x => x.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);

        // Aynı dil içinde aynı inflection type tekrar eklenemesin
        builder.HasIndex(x => new { x.LanguageId, x.Name })
            .IsUnique();

        // Arama için
        builder.HasIndex(x => x.Name);
    }
}