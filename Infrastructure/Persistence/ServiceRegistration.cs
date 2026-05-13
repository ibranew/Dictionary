using Dictionary.Application.Common.Interfaces;
using Dictionary.Domain.Entities.Identity;
using Dictionary.Persistence.Context;
using Dictionary.Persistence.Context.Interceptors;
using Dictionary.Persistence.Identity;
using Dictionary.Persistence.Options;
using Microsoft.Extensions.Options;
using Dictionary.Persistence.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Dictionary.Persistence;
public static class ServiceRegistration
{
    /// <summary>
    ///  Api projesinin Program.cs dosyasında çağrılarak, 
    ///  Persistence katmanındaki servislerin DI container'a eklenmesini sağlar.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <exception cref="Exception"></exception>
    // Dictionary.Persistence/ServiceRegistration.cs
    public static void AddPersistenceServicesForApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var activeName = configuration["DatabaseSettings:ActiveConnection"];
        if (string.IsNullOrWhiteSpace(activeName))
            throw new Exception("DatabaseSettings:ActiveConnection boş!");

        var connectionString = configuration.GetConnectionString(activeName);
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new Exception($"Connection string bulunamadı: {activeName}");

        services.AddScoped<SoftDeleteInterceptor>();



        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString);
            var interceptor = sp.GetRequiredService<SoftDeleteInterceptor>();
            options.AddInterceptors(interceptor);
        });

        services.AddScoped<IAppDbContext>(provider =>
            provider.GetRequiredService<AppDbContext>());

       

        services.AddIdentityCore<AppUser>(options =>
        {
            options.Password.RequiredLength = 6;// Minimum 6 karakter.
            options.Password.RequireDigit = false; // Rakam zorunlu değil.
            options.Password.RequireLowercase = true; // Küçük harf zorunlu.
            options.Password.RequireUppercase = false;// Büyük harf zorunlu değil.
            options.Password.RequireNonAlphanumeric = false;// Özel karakter zorunlu değil.

            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

            options.User.RequireUniqueEmail = true;
        })
         .AddRoles<AppRole>()
         .AddEntityFrameworkStores<AppDbContext>()
         .AddSignInManager()
         .AddDefaultTokenProviders();

        //Admin
        services.Configure<SeedAdminSettings>(
                 configuration.GetSection(SeedAdminSettings.SectionName));

        services.AddScoped<ILookupService, LookupService>();
    }
}
