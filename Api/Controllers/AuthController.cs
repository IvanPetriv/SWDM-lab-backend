//using AutoMapper;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace api.Controllers.Auth;

//// TODO: centralize length of refresh token in app settings

//[AllowAnonymous]
//[Route("api/v1/[controller]")]
//[ApiController]
//public class AuthController(AuthService authService, IMapper mapper) : ControllerBase {
//    private const int RefreshTokenSeconds = 86400;

//    /// <summary>
//    /// Gets the refresh token from the HTTP cookies.
//    /// </summary>
//    /// <returns></returns>
//    private string? GetRefreshCookie() {
//        return Request.Cookies["refreshToken"];
//    }

//    /// <summary>
//    /// Sets the refresh token in the HTTP cookies.
//    /// </summary>
//    /// <param name="refreshToken">Refresh token to set.</param>
//    private void SetRefreshCookie(string refreshToken) {
//        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions {
//            HttpOnly = true,
//            Secure = true,
//            SameSite = SameSiteMode.None,
//            Expires = DateTimeOffset.UtcNow.AddSeconds(RefreshTokenSeconds)
//        });
//    }

//    [HttpPost("signup")]
//    public async Task<ActionResult<AccessTokenResult>> Signup([FromBody] ManualAuthUser dto, CancellationToken ct) {
//        var result = await authService.SignupUserAsync(dto, ct);
//        var response = mapper.Map<UserGetDto>(result);
//        return Ok(response);
//    }


//    [HttpPost("login")]
//    public async Task<ActionResult<AccessTokenResult>> Login([FromBody] LoginRequest dto, CancellationToken ct) {
//        var result = await authService.LoginManuallyAsync(dto.Login, dto.Password, ct);
//        if (result is null) {
//            return NotFound();
//        }

//        SetRefreshCookie(result.RefreshToken);

//        return Ok(new AccessTokenResult(result.AccessToken));
//    }


//    [HttpGet("refresh")]
//    public async Task<ActionResult<AccessTokenResult>> Refresh(CancellationToken ct) {
//        string? refreshToken = GetRefreshCookie();
//        if (string.IsNullOrEmpty(refreshToken)) {
//            return Unauthorized();
//        }

//        // Refreshes the token in application service and returns a new one
//        var result = await authService.RefreshTokenAsync(refreshToken, ct);
//        if (result is null) {
//            return Unauthorized();
//        }

//        SetRefreshCookie(result.RefreshToken);

//        return Ok(new AccessTokenResult(result.AccessToken));
//    }
//}
