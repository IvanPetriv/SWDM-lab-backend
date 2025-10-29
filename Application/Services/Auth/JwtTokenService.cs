using Application.Objects.Auth;
using Application.Objects.Configurations;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services.Auth;
public class JwtTokenService(JwtConfiguration jwtConfig) {
    /// <summary>
    /// Generates a JWT token for a specified user for a specified period of time.
    /// </summary>
    /// <param name="user">User claims to be included in the token.</param>
    /// <param name="expiresInSeconds">Period of time for which the token will be valid.</param>
    /// <returns>Generated JWT token.</returns>
    /// <exception cref="ArgumentException">Thrown if the secret key is too short (<32 characters)</exception>
    public string GetJwtToken(JwtUserClaims user, int expiresInSeconds) {
        // Checks if configuration is correct
        if (string.IsNullOrEmpty(jwtConfig.Key) || jwtConfig.Key.Length < 32) {
            throw new ArgumentException("JWT key must be at least 32 characters long.");
        }

        // Prepares the data for the token
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(jwtConfig.Key));
        SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);
        List<Claim> claims = [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.PublicId),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
        ];

        // Creates the token
        JwtSecurityToken token = new(jwtConfig.Issuer,
                                    jwtConfig.Audience,
                                    claims,
                                    expires: DateTime.Now.AddSeconds(expiresInSeconds),
                                    signingCredentials: signingCredentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
