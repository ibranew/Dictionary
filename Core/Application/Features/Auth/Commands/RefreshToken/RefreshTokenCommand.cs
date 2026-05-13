using Dictionary.Application.Common;
using Dictionary.Application.Common.DTOs.Auth;
using Dictionary.Application.Common.Interfaces;
using Dictionary.Application.Common.Interfaces.Auth;
using Dictionary.Application.Features.Auth.DTOs;
using Dictionary.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictionary.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<AppResult<AuthResponseDto>>
    {
        public string? RefreshToken { get; set; }
        public string IpAddress { get; set; } = null!;
    }
    public class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, AppResult<AuthResponseDto>>
    {
        private readonly IAppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;

        public RefreshTokenCommandHandler(
            IAppDbContext context,
            UserManager<AppUser> userManager,
            ITokenService tokenService)
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<AppResult<AuthResponseDto>> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Cookie'den gelen refresh token kontrolü
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return AppResult<AuthResponseDto>
                    .Failure("Refresh token bulunamadı.");

            // 2. Refresh token veritabanında var mı kontrol et
            var existingToken = _context.RefreshTokens
                .SingleOrDefault(x => x.Token == request.RefreshToken);

            // 3. Token geçerli mi kontrol et
            if (existingToken is null || !existingToken.IsActive)
                return AppResult<AuthResponseDto>
                    .Failure("Geçersiz veya süresi dolmuş token.");

            // 4. Kullanıcıyı bul
            var user = await _userManager.FindByIdAsync(existingToken.UserId.ToString());

            // 5. Kullanıcı aktif mi kontrol et
            if (user is null || !user.IsActive)
                return AppResult<AuthResponseDto>
                    .Failure("Kullanıcı bulunamadı. ya da hesabınız aktif değil.");

            // 6. Yeni refresh token üret
            var newRefreshToken = _tokenService.GenerateRefreshToken(request.IpAddress);
            newRefreshToken.UserId = user.Id;

            // 7. Eski token'ı revoke et (Token Rotation)
            existingToken.RevokedAt = DateTime.UtcNow;
            existingToken.RevokedByIp = request.IpAddress;
            existingToken.ReplacedByToken = newRefreshToken.Token;

            // 8. Yeni refresh token'ı kaydet
            _context.RefreshTokens.Add(newRefreshToken);

            // 9. Kullanıcı rollerini al
            var roles = await _userManager.GetRolesAsync(user);

            // 10. Yeni access token üret
            var accessToken = _tokenService.GenerateAccessToken(user, roles);

            // 11. Veritabanı değişikliklerini kaydet
            await _context.SaveChangesAsync(cancellationToken);

            // 12. Response oluştur
            var response = new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiresAt = newRefreshToken.ExpiresAt
            };

            // 13. Başarılı sonucu dön
            return AppResult<AuthResponseDto>.Success(response);
        }
    }
}
