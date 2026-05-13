using Dictionary.Application.Common.Interfaces;
using Dictionary.Domain.Entities;
using Dictionary.Domain.Entities.Identity;
using Dictionary.Persistence.Context.Interceptors;
using Dictionary.Persistence.Context.ModelBuilderExtensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Dictionary.Persistence.Context;

// ═══════════════════════════════════════════════════════════
// APP DB CONTEXT
// ═══════════════════════════════════════════════════════════

/// <summary>
/// Uygulamanın ana veritabanı context'i.
/// Configuration'lar otomatik olarak assembly üzerinden taranır.
/// </summary>
public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Language> Languages { get; set; }
    public DbSet<ExampleTranslation> ExampleTranslations { get; set; }
    public DbSet<Entry> Entries { get; set; }
    public DbSet<EntryForm> EntryForms { get; set; }
    public DbSet<Sense> Senses { get; set; }
    public DbSet<SenseTranslation> SenseTranslations { get; set; }
    public DbSet<SenseDefinition> SenseDefinitions { get; set; }
    public DbSet<Label> Labels { get; set; }
    public DbSet<SenseLabel> SenseLabels { get; set; }
    public DbSet<Example> Examples { get; set; }
    public DbSet<SenseHistory> SenseHistories { get; set; }
    public DbSet<Etymology> Etymologies { get; set; }
    public DbSet<SenseRelation> SenseRelations { get; set; }
    public DbSet<LabelType> LabelTypes { get; set; }
    public DbSet<Script> Scripts { get; set; }
    public DbSet<InflectionType> InflectionTypes { get; set; }
    public DbSet<Pronunciation> Pronunciations { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // EntityConfigurations klasöründeki tüm IEntityTypeConfiguration'ları otomatik uygula
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        
        //SoftDelete
        modelBuilder.ApplySoftDeleteFilter();
    }

}
