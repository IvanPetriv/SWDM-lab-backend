namespace Application.Objects.Auth;

public record ManualAuthUser(
    string Email,
    string Username,
    string Password,
    string? FirstName,
    string? LastName,
    string? Role = "Student"
);
