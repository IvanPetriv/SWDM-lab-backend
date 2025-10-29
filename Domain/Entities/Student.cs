using System.Text.RegularExpressions;

namespace Domain.Entities;
public partial class Student {
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    private string email;
    public string Email {
        get => email;
        set {
            if (string.IsNullOrWhiteSpace(value) || !EmailRegex().IsMatch(value)) {
                throw new ArgumentException("Invalid email format.", nameof(value));
            }

            email = value;
        }
    }
    public virtual ICollection<Enrollment> Enrollments { get; set; } = [];



    [GeneratedRegex(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex EmailRegex();
}
