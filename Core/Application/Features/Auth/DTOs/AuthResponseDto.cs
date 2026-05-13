using Dictionary.Application.Common.DTOs.Auth;


namespace Dictionary.Application.Features.Auth.DTOs
{
    public class AuthResponseDto
    {
        public TokenDto Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public DateTime RefreshTokenExpiresAt { get; set; }
    }
}
