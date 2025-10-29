namespace Application.Objects.Auth;

public record AuthResult(
    string AccessToken,
    string RefreshToken
);

