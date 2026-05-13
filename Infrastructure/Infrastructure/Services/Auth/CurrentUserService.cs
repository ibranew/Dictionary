// Dictionary.Infrastructure/Services/CurrentUserService.cs
using Dictionary.Application.Common.Interfaces;
using global::Dictionary.Application.Common.Interfaces.Auth;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Dictionary.Infrastructure.Services.Auth;
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public int? UserId
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User?.FindFirstValue("sub");

            return int.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email => User?.FindFirstValue(ClaimTypes.Email)
                         ?? User?.FindFirstValue("email");

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;
}
