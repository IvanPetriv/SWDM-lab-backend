using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Configurations;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Auth;

public class JwtTokenService(JwtConfiguration jwtConfig)
{
    private static string GetUserRole(User user)
    {
        return user switch
        {
            Student => "Student",
            Administrator => "Administrator",
            Teacher => "Teacher",
            _ => throw new ArgumentException("Unknown user type")
        };
    }

    public string GetJwtToken(User user)
    {
        if (string.IsNullOrEmpty(jwtConfig.Secret) || jwtConfig.Secret.Length < 32)
            throw new ArgumentException("JWT key must be at least 32 characters long.");

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(jwtConfig.Secret));
        SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(ClaimTypes.Role, GetUserRole(user)),
        ];

        JwtSecurityToken token = new(
            jwtConfig.Issuer,
            jwtConfig.Audience,
            claims,
            expires: DateTime.Now.AddSeconds(jwtConfig.Expiry),
            signingCredentials: signingCredentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}