using Dictionary.Application.Common.DTOs.Auth;
using Dictionary.Domain.Entities.Identity;
namespace Dictionary.Application.Common.Interfaces.Auth;

public interface ITokenService
{
    /// <summary>
    /// Kullanıcı için JWT Access Token üretir.
    /// </summary>
    TokenDto GenerateAccessToken(AppUser user, IList<string> roles);

    /// <summary>
    /// Kriptografik olarak güvenli Refresh Token üretir.
    /// </summary>
    RefreshToken GenerateRefreshToken(string ipAddress);
}