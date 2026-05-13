using Dictionary.Application.Common.DTOs.Auth;
using Dictionary.Application.Common.Interfaces.Auth;
using Dictionary.Domain.Entities.Identity;
using Dictionary.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
namespace Dictionary.Infrastructure.Services.Auth;
public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public TokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public TokenDto GenerateAccessToken(AppUser user, IList<string> roles)
    {
        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

        var claims = BuildClaims(user, roles);
        var signingKey = BuildSigningKey();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256Signature)
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);

        return new TokenDto(handler.WriteToken(token), expires);
    }

    public RefreshToken GenerateRefreshToken(string ipAddress)
    {
        // Kriptografik olarak güvenli rastgele token
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };
    }

    // ─────────────────────────────────────────────
    // Private Helpers
    // ─────────────────────────────────────────────

    private static List<Claim> BuildClaims(AppUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // token unique id
            new("username", user.UserName ?? string.Empty),
            new("fullName", user.FullName ?? string.Empty),
        };

        // Her role için ayrı claim ekle
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));
        return claims;
    }

    private SymmetricSecurityKey BuildSigningKey()
    {
        var keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
        return new SymmetricSecurityKey(keyBytes);
    }
}
