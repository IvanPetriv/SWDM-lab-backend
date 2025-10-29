using Api.Dtos;
using Application.Objects.Auth;
using Application.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(AuthService authService) : ControllerBase
{
    private const int RefreshTokenSeconds = 86400;

    private void SetRefreshCookie(string refreshToken)
    {
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddSeconds(RefreshTokenSeconds),
        });
    }

    private string? GetRefreshCookie()
    {
        return Request.Cookies["refreshToken"];
    }

    [HttpPost("signup")]
    public async Task<ActionResult<UserGetDto>> Signup([FromBody] ManualAuthUser dto, CancellationToken ct)
    {
        var result = await authService.SignupUserAsync(dto, ct);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AccessTokenResult>> Login([FromBody] LoginRequest dto, CancellationToken ct)
    {
        var result = await authService.LoginManuallyAsync(dto.Email, dto.Password, ct);
        if (result is null) return Unauthorized();

        SetRefreshCookie(result.RefreshToken);
        return Ok(new AccessTokenResult(result.AccessToken));
    }

    [HttpGet("refresh")]
    public async Task<ActionResult<AccessTokenResult>> Refresh(CancellationToken ct)
    {
        var refreshToken = GetRefreshCookie();
        var result = await authService.RefreshTokenAsync(refreshToken, ct);
        if (result is null) return Unauthorized();

        SetRefreshCookie(result.RefreshToken);
        return Ok(new AccessTokenResult(result.AccessToken));
    }
}

public record LoginRequest(string Email, string Password);