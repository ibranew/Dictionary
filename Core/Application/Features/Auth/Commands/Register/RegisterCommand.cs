using Dictionary.Application.Common;
using Dictionary.Application.Common.Interfaces;
using Dictionary.Application.Common.Interfaces.Auth;
using Dictionary.Application.Features.Auth.DTOs;
using Dictionary.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Dictionary.Application.Features.Auth.Commands.Register;

public class RegisterCommand : IRequest<AppResult>
{
    public RegisterDto RegisterDto { get; set; } = null!;
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AppResult>
{
    private readonly UserManager<AppUser> _userManager;
    public RegisterCommandHandler(UserManager<AppUser> userManager)
    {
      
        _userManager=userManager;
    }
    public async Task<AppResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var dto = request.RegisterDto;
        // Yeni kullanıcı 
        var user = new AppUser
        {
            Email = dto.Email,
            UserName = dto.UserName,
            FullName = dto.FullName
        };
        var result = await _userManager.CreateAsync(user, dto.Password);

        //result'tan gelen hatalar ingilizce olabilir,
        //bu yüzden onları Türkçeye çevirebiliriz
        //veya direkt olarak kullanıcıya göstermek yerine genel bir hata mesajı verebiliriz.

        return result.Succeeded
            ? AppResult.Success("Kullanıcı başarıyla kaydedildi.")
            : AppResult.Failure(result.Errors.Select(e => e.Description).ToList());
    }
}
