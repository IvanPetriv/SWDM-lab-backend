namespace Application.Objects.Configurations;

/// <summary>
/// JWT provider configuration settings, including issuer, audience, and secret key.
/// </summary>
/// <param name="Issuer"></param>
/// <param name="Audience"></param>
/// <param name="Key"></param>
public record JwtConfiguration(string Issuer, string Audience, string Key);
