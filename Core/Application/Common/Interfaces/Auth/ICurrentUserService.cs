
namespace Dictionary.Application.Common.Interfaces.Auth;
public interface ICurrentUserService
{
    int? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}