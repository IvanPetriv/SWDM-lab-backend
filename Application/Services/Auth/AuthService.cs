using Application.Objects.Auth;
using Application.Services.Users;
using Domain.Entities;

namespace Application.Services.Auth;

public class AuthService(
    IAuthUserService userService,
    JwtTokenService jwtTokenService,
    RefreshTokenService refreshTokenService
)
{
    private async Task<AuthResult> GenerateTokensAsync(User user, CancellationToken ct)
    {
        var jwtToken = jwtTokenService.GetJwtToken(user);
        var refreshToken = refreshTokenService.GenerateRefreshToken();
        await refreshTokenService.SaveRefreshToken(refreshToken, user, ct);

        return new(jwtToken, refreshToken);
    }

    public async Task<AuthResult?> LoginManuallyAsync(string email, string password, CancellationToken ct)
    {
        var user = await userService.GetByEmailAsync(email, ct);
        if (user?.PasswordHash is null) return null;

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        return isPasswordValid ? await GenerateTokensAsync(user, ct) : null;
    }

    public async Task<AuthResult?> RefreshTokenAsync(string? refreshToken, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(refreshToken)) return null;

        var isValid = await refreshTokenService.IsTokenValid(refreshToken, ct);
        if (!isValid) return null;

        var refreshTokenObj = await refreshTokenService.GetTokenAsync(refreshToken, ct);
        if (refreshTokenObj is null) return null;

        var user = await userService.GetAsync(refreshTokenObj.UserId, ct);
        if (user is null) return null;

        return await GenerateTokensAsync(user, ct);
    }

    public async Task<User> SignupUserAsync(ManualAuthUser authUser, CancellationToken ct)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(authUser.Password);

        User user = (authUser.Role?.ToLowerInvariant()) switch
        {
            "administrator" or "admin" => new Administrator
            {
                Email = authUser.Email,
                Username = authUser.Username,
                FirstName = authUser.FirstName ?? "",
                LastName = authUser.LastName ?? "",
                PasswordHash = hashedPassword
            },
            "teacher" => new Teacher
            {
                Email = authUser.Email,
                Username = authUser.Username,
                FirstName = authUser.FirstName ?? "",
                LastName = authUser.LastName ?? "",
                PasswordHash = hashedPassword
            },
            _ => new Student
            {
                Email = authUser.Email,
                Username = authUser.Username,
                FirstName = authUser.FirstName ?? "",
                LastName = authUser.LastName ?? "",
                PasswordHash = hashedPassword
            }
        };

        return await userService.CreateAsync(user, ct);
    }
}