using System.Text.RegularExpressions;

namespace Domain.Entities;

public abstract class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    private string email = string.Empty;
    public string Email
    {
        get => email;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || !EmailRegex.IsMatch(value))
            {
                throw new ArgumentException("Invalid email format.", nameof(value));
            }

            email = value;
        }
    }

    // Navigations
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<Enrollment> Enrollments { get; set; } = [];


    private static readonly Regex EmailRegex = new(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
}
