
namespace Dictionary.Application.Common.DTOs.Auth;
public record TokenDto(
    string AccessToken,
    DateTime AccessTokenExpires
);