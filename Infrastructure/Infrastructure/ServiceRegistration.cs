
using Dictionary.Application.Common.Interfaces.Auth;
using Dictionary.Application.Common.Interfaces.Text;
using Dictionary.Infrastructure.Options;
using Dictionary.Infrastructure.Services.Auth;
using Dictionary.Infrastructure.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Dictionary.Infrastructure;

public static class ServiceRegistration
{
    public static void AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // JwtSettings'i Options pattern ile bağla
        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

        // Token ve CurrentUser servisleri
        services.AddScoped<ITokenService, TokenService>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // JWT Authentication
        var jwtSettings = configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>()!;

        var keyBytes = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

                // Token süresi dolunca hemen geçersiz say
                ClockSkew = TimeSpan.Zero,
            };

           

            // 401 ve 403 için response detayı
            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync(
                        """{"error": "Yetkisiz erişim. Lütfen giriş yapın."}""");
                },
                OnForbidden = context =>
                {
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync(
                        """{"error": "Bu kaynağa erişim yetkiniz yok."}""");
                }
            };
        });

        services.AddScoped<ITextNormalizer,TurkishTextNormalizer>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
    }
}
