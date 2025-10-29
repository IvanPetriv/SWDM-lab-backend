using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.Entities;
public abstract class User {
    public Guid Id { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";

    private string email;
    public string Email {
        get => email;
        set {
            if (string.IsNullOrWhiteSpace(value) || !EmailRegex.IsMatch(value)) {
                throw new ArgumentException("Invalid email format.", nameof(value));
            }

            email = value;
        }
    }


    private static readonly Regex EmailRegex = new(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
}
