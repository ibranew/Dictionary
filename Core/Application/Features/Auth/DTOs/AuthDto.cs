
namespace Dictionary.Application.Features.Auth.DTOs;
public class RegisterDto
{
    public string FullName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
public class LoginDto
{
    public string EmailOrUserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool RememberMe { get; set; } = false;
}