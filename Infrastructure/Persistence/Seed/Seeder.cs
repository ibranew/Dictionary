using Dictionary.Application.Common.Authorization;
using Dictionary.Application.Common.Interfaces;
using Dictionary.Domain.Entities;
using Dictionary.Domain.Entities.Identity;
using Dictionary.Persistence.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dictionary.Persistence.Seed;

public static class Seeder
{
    public static async Task SeedAll(IServiceProvider sp)
    {
        await SeedLanguages(sp);
        await SeedScripts(sp);  
        await SeedLabelTypes(sp);
        await SeedLabels(sp);
        await SeedInflectionTypes(sp);

     
        await SeedSampleData(sp);
    }

    public static async Task SeedLanguages(IServiceProvider sp)
    {
        var context = sp.GetRequiredService<IAppDbContext>();

        if (!context.Languages.Any())
        {
            var languages = new List<Language>
            {
              new Language { Code = "tr", Name = "Türkçe" },
              new Language { Code = "ko", Name = "Korece" },
            };

            context.Languages.AddRange(languages);
            await context.SaveChangesAsync(CancellationToken.None);
        }
    }

    public static async Task SeedScripts(IServiceProvider sp)
    {
        var context = sp.GetRequiredService<IAppDbContext>();

        if (!context.Scripts.Any())
        {
            var scripts = new List<Script>
            {
              new Script { Code = "latin", Name = "Latin" },
              new Script { Code = "hangul", Name = "Hangul" },
              new Script { Code = "hanja", Name = "Hanja" }
            };

            context.Scripts.AddRange(scripts);
            await context.SaveChangesAsync(CancellationToken.None);
        }
    }

    public static async Task SeedLabelTypes(IServiceProvider sp)
    {
        var context = sp.GetRequiredService<IAppDbContext>();

        if (!context.LabelTypes.Any())
        {
            var labelTypes = new List<LabelType>
        {
            new LabelType { Code = "register", DisplayName = "Kullanım Biçimi" },
            new LabelType { Code = "usage", DisplayName = "Kullanım Alanı" },
            new LabelType { Code = "region", DisplayName = "Bölgesel" },
            new LabelType { Code = "historical", DisplayName = "Tarihsel Dönem" },
            new LabelType { Code = "other", DisplayName = "Diğer" }
        };

            context.LabelTypes.AddRange(labelTypes);
            await context.SaveChangesAsync(CancellationToken.None);
        }
    }

    public static async Task SeedLabels(IServiceProvider sp)
    {
        var context = sp.GetRequiredService<IAppDbContext>();

        if (!context.Labels.Any())
        {
            var registerType = context.LabelTypes.First(x => x.Code == "register");
            var usageType = context.LabelTypes.First(x => x.Code == "usage");
            var regionType = context.LabelTypes.First(x => x.Code == "region");

            var labels = new List<Label>
            {
            // REGISTER
            new Label { Name = "Resmi", LabelTypeId = registerType.Id },
            new Label { Name = "Gündelik", LabelTypeId = registerType.Id },
            new Label { Name = "Argo", LabelTypeId = registerType.Id },

            // USAGE
            new Label { Name = "Tıp", LabelTypeId = usageType.Id },
            new Label { Name = "Hukuk", LabelTypeId = usageType.Id },
            new Label { Name = "Bilişim", LabelTypeId = usageType.Id },

            // REGION
            new Label { Name = "Türkiye Türkçesi", LabelTypeId = regionType.Id },
            new Label { Name = "Güney Kore", LabelTypeId = regionType.Id }
            };

            context.Labels.AddRange(labels);
            await context.SaveChangesAsync(CancellationToken.None);
        }
    }

    public static async Task SeedInflectionTypes(IServiceProvider sp)
    {
        var context = sp.GetRequiredService<IAppDbContext>();

        if (!context.InflectionTypes.Any())
        {
            var tr = context.Languages.First(x => x.Code == "tr");
            var ko = context.Languages.First(x => x.Code == "ko");

            var list = new List<InflectionType>
            {
            // Türkçe
            new InflectionType { Name = "Geçmiş Zaman", LanguageId = tr.Id },
            new InflectionType { Name = "Şimdiki Zaman", LanguageId = tr.Id },

            // Korece
            new InflectionType { Name = "Past", LanguageId = ko.Id },
            new InflectionType { Name = "Polite", LanguageId = ko.Id },
            new InflectionType { Name = "Honorific", LanguageId = ko.Id }
            };

            context.InflectionTypes.AddRange(list);
            await context.SaveChangesAsync(CancellationToken.None);
        }
    }

    public static async Task SeedSampleData(IServiceProvider sp)
    {
        var context = sp.GetRequiredService<IAppDbContext>();

        if (!context.Entries.Any())
        {
            var tr = context.Languages.First(x => x.Code == "tr");
            var ko = context.Languages.First(x => x.Code == "ko");

            var scriptLatin = context.Scripts.First(x => x.Code == "latin");
            var scriptHangul = context.Scripts.First(x => x.Code == "hangul");
            var scriptHanja = context.Scripts.First(x => x.Code == "hanja");

            // ENTRY (Korece 먹다)
            var entryKo = new Entry
            {
                Headword = "먹다",
                HeadwordNormalized = "먹다",
                LanguageId = ko.Id,
                ScriptId = scriptHangul.Id
            };

            var senseKo = new Sense
            {
                Entry = entryKo,
                PartOfSpeechType = PartOfSpeechType.Verb,
                Order = 1,
                Definitions = new List<SenseDefinition>
                {
                 new SenseDefinition
                 {
                    LanguageId = ko.Id,
                    Text = "음식을 섭취하다"
                 }
                }
            };

            // ENTRY (Türkçe yemek)
            var entryTr = new Entry
            {
                Headword = "Yemek",
                HeadwordNormalized = "yemek",
                LanguageId = tr.Id,
                ScriptId = scriptLatin.Id
            };

            var senseTr = new Sense
            {
                Entry = entryTr,
                PartOfSpeechType = PartOfSpeechType.Verb,
                Order = 1,
                Definitions = new List<SenseDefinition>
            {
                new SenseDefinition
                {
                    LanguageId = tr.Id,
                    Text = "Besin tüketmek"
                }
            }
            };

            // Translation
            var translation = new SenseTranslation
            {
                SourceSense = senseKo,
                TargetSense = senseTr,
                Confidence = 5,
                Note = "Genel kullanım"
            };

            context.AddRange(entryKo, entryTr, translation);

            await context.SaveChangesAsync(CancellationToken.None);
        }
    }

    public static async Task ResetDatabase(IServiceProvider sp)
    {
        var context = sp.GetRequiredService<IAppDbContext>();

        //Tüm tabloları sıfırla (dependency sırasına dikkat)
        await context.SenseTranslations.ExecuteDeleteAsync();
        await context.SenseLabels.ExecuteDeleteAsync();
        await context.SenseDefinitions.ExecuteDeleteAsync();
        await context.SenseRelations.ExecuteDeleteAsync();
        await context.SenseHistories.ExecuteDeleteAsync();

        await context.Senses.ExecuteDeleteAsync();
        await context.Examples.ExecuteDeleteAsync();
        await context.ExampleTranslations.ExecuteDeleteAsync();

        await context.Entries.ExecuteDeleteAsync();
        await context.Labels.ExecuteDeleteAsync();
        await context.LabelTypes.ExecuteDeleteAsync();
        await context.InflectionTypes.ExecuteDeleteAsync();

        await context.Scripts.ExecuteDeleteAsync();
        await context.Languages.ExecuteDeleteAsync();

        await context.SaveChangesAsync(CancellationToken.None);
    }

    public static async Task SeedRolesAndAdmin(IServiceProvider sp)
    {
        var roleManager = sp.GetRequiredService<RoleManager<AppRole>>();
        var userManager = sp.GetRequiredService<UserManager<AppUser>>();

        // ROLES
        string[] roles = {DictionaryRoles.Admin, DictionaryRoles.Editor};

        foreach (var roleName in roles)
        {
            bool roleExists = await roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                await roleManager.CreateAsync(new AppRole
                {
                    Name = roleName
                });
            }
        }

        // ADMIN USER
        SeedAdminSettings adminSettings = sp.GetRequiredService<IOptions<SeedAdminSettings>>().Value;
        // Admin bilgilerini appsettings.json'dan al
        string adminEmail = adminSettings.Email;
        string adminPassword = adminSettings.Password;
        string adminUserName = adminSettings.UserName;

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser is null)
        {
            adminUser = new AppUser
            {
                UserName = adminUserName,
                Email = adminEmail,
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, DictionaryRoles.Admin);
            }
        }
    }

}