using Dictionary.Application.Common;
using Dictionary.Application.Common.DTOs.Auth;
using Dictionary.Application.Common.Interfaces;
using Dictionary.Application.Common.Interfaces.Auth;
using Dictionary.Application.Features.Auth.DTOs;
using Dictionary.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Dictionary.Application.Features.Auth.Commands.Login;

public class LoginCommand : IRequest<AppResult<AuthResponseDto>>
{
    public LoginDto LoginDto { get; set; } = null!;
    public string IpAdress { get; set; } = null!;
}
public class LoginCommandHandler : IRequestHandler<LoginCommand, AppResult<AuthResponseDto>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IAppDbContext _context;

    public LoginCommandHandler(IAppDbContext context, ITokenService tokenService, UserManager<AppUser> userManager)
    {
        _context=context;
        _tokenService=tokenService;
        _userManager=userManager;
    }
    public async Task<AppResult<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var dto = request.LoginDto;

        // 1. Kullanıcıyı email üzerinden bul
        var user = await _userManager.FindByEmailAsync(dto.EmailOrUserName);

        // 1.1 Eğer email ile bulunamazsa, kullanıcı adını dene
        if (user is null)
            user = await _userManager.FindByNameAsync(dto.EmailOrUserName);

        // 2. Kullanıcı yoksa veya şifre yanlışsa hata dön
        if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return AppResult<AuthResponseDto>.Failure("E-posta veya şifre hatalı.");

        // 3. Kullanıcı aktif mi kontrol et
        if (!user.IsActive)
            return AppResult<AuthResponseDto>.Failure("Hesabınız aktif değil.");

        // 4. Kullanıcının rollerini çek
        var roles = await _userManager.GetRolesAsync(user);

        // 5. Access token üret
        var accessToken = _tokenService.GenerateAccessToken(user, roles);

        // 6. Refresh token üret
        var refreshToken = _tokenService.GenerateRefreshToken(request.IpAdress);
        refreshToken.UserId = user.Id;

        // 7. Refresh token'ı veritabanına kaydet
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        // 8. Response oluştur
        var response = new AuthResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt
        };

        // 9. Başarılı sonucu dön
        return AppResult<AuthResponseDto>.Success(response);
    }
}

