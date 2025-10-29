using Application.Objects.Configurations;
using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services.Auth;
public class RefreshTokenService(RefreshTokenConfiguration config, UniversityDbContext dbContext) {
    public static string HashToken(string token) {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }

    public async Task<RefreshToken?> GetTokenAsync(string token, CancellationToken ct) {
        string hashedRefreshToken = HashToken(token);
        RefreshToken? retrievedToken = await dbContext.RefreshTokens
           .AsNoTracking()
           .FirstOrDefaultAsync(x => x.HashedToken == hashedRefreshToken, ct);
        if (retrievedToken is null) {
            return null;
        }
        return retrievedToken;
    }


    public string GenerateRefreshToken() {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public async Task SaveRefreshToken(string refreshToken, User user, CancellationToken ct) {
        RefreshToken refreshTokenObj = new() {
            UserId = user.Id,
            HashedToken = HashToken(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddSeconds(config.DurationInSeconds),
            CreatedAt = DateTime.UtcNow,
        };

        await dbContext.RefreshTokens.AddAsync(refreshTokenObj, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> IsTokenValid(string refreshToken, CancellationToken ct) {
        string hashedRefreshToken = HashToken(refreshToken);
        RefreshToken? storedToken = await dbContext.RefreshTokens
           .AsNoTracking()
           .FirstOrDefaultAsync(x => x.HashedToken == hashedRefreshToken, ct);
        if (storedToken is null) {
            return false;
        }

        bool isNotExpired = storedToken.ExpiresAt > DateTime.UtcNow;
        bool isNotRevoked = storedToken.RevokedAt is null;

        return isNotExpired && isNotRevoked;
    }

    public async Task RevokeToken(string refreshToken, CancellationToken ct) {
        string hashedRefreshToken = HashToken(refreshToken);
        var retrievedToken = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.HashedToken == hashedRefreshToken, ct);

        if (retrievedToken is not null) {
            retrievedToken.RevokedAt = DateTime.UtcNow;
        }
        await dbContext.SaveChangesAsync(ct);
    }
}
