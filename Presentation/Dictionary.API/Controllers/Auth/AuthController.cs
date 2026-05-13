using Dictionary.API.Controllers.Common;
using Dictionary.Application.Common;
using Dictionary.Application.Features.Auth.Commands.Login;
using Dictionary.Application.Features.Auth.Commands.Logout;
using Dictionary.Application.Features.Auth.Commands.RefreshToken;
using Dictionary.Application.Features.Auth.Commands.Register;
using Dictionary.Application.Features.Auth.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Dictionary.API.Controllers.Auth;

//[Authorize(Policy = Policies.AdminOnly)]
[Route("api/auth")]
[ApiController]
public class AuthController : DictionaryControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var cmd = new RegisterCommand() { RegisterDto = dto};
        AppResult appResult = await Sender.Send(cmd);
        
        if(appResult.IsSuccess)
        {
            return Ok(appResult);
        }
        return BadRequest(appResult);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var cmd = new LoginCommand { LoginDto = dto, IpAdress = GetIpAddress() };

        var result = await Sender.Send(cmd);

        if (!result.IsSuccess)
            return Unauthorized(result); 

        var data = result.Data;

        if(data == null)
            return Unauthorized(result);

        //
        SetRefreshTokenCookie(data.RefreshToken, data.RefreshTokenExpiresAt);

        //todo
        data.RefreshToken = "[REDACTED]";
        data.RefreshTokenExpiresAt = DateTime.MinValue;

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var cookieToken = Request.Cookies["refreshToken"];
        var cmd = new RefreshTokenCommand() { RefreshToken = cookieToken, IpAddress = GetIpAddress() };
        var appResult = await Sender.Send(cmd);

        if (appResult.IsSuccess)
        {
            var refreshResponse = appResult.Data;
            if (refreshResponse == null)
            {
                return BadRequest(appResult);
            }

            SetRefreshTokenCookie(refreshResponse.RefreshToken, refreshResponse.RefreshTokenExpiresAt);
            return Ok(appResult);
        }

        return BadRequest(appResult);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var command = new LogoutCommand
        {
            RefreshToken = Request.Cookies["refreshToken"],
            IpAddress = GetIpAddress()
        };
        var result = await Sender.Send(command);
        // Cookie'yi sil
        Response.Cookies.Delete("refreshToken");

        return Ok(result);
    }
    private string GetIpAddress()
    {
        if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            return forwarded.ToString();

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
    private void SetRefreshTokenCookie(string token, DateTime expires)
    {
        //Refresh token'ı HttpOnly cookie olarak ayarla

        Response.Cookies.Append("refreshToken", token, new CookieOptions
        {
            HttpOnly = true,       // JS erişemez → XSS koruması
            Secure = true,         // Sadece HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = expires
        });
    }
}
