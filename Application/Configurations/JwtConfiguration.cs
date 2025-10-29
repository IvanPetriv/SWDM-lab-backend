namespace Application.Configurations;

public record JwtConfiguration(string Secret, string Issuer, string Audience, int Expiry);