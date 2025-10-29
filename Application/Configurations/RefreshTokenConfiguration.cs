namespace Application.Objects.Configurations;

/// <summary>
/// JWT provider configuration settings, including issuer, audience, and secret key.
/// </summary>
/// <param name="Issuer"></param>
/// <param name="Audience"></param>
/// <param name="Key"></param>
public record RefreshTokenConfiguration {
    public int DurationInSeconds { get; set; }
    public int Length { get; set; }
}
