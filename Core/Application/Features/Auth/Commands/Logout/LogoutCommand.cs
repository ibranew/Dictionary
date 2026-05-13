using Dictionary.Application.Common;
using Dictionary.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Dictionary.Application.Features.Auth.Commands.Logout;

public class LogoutCommand : IRequest<AppResult>
{
    public string? RefreshToken { get; set; }
    public string IpAddress { get; set; } = null!;
}
public class LogoutCommandHandler : IRequestHandler<LogoutCommand, AppResult>
{
    private readonly IAppDbContext _context;
    public LogoutCommandHandler(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<AppResult> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Refresh token boşsa direkt başarılı dön
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return AppResult.Success("Çıkış başarılı.");

        // 2. Refresh token veritabanında var mı kontrol et
        var existingToken = await _context.RefreshTokens
            .SingleOrDefaultAsync(
                x => x.Token == request.RefreshToken,
                cancellationToken);

        // 3. Token aktifse revoke et
        if (existingToken?.IsActive == true)
        {
            existingToken.RevokedAt = DateTime.UtcNow;
            existingToken.RevokedByIp = request.IpAddress;

            await _context.SaveChangesAsync(cancellationToken);
        }

        // 4. Başarılı sonucu dön
        return AppResult.Success("Çıkış başarılı.");
    }
}