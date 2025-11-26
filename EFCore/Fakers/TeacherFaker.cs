using Bogus;
using Domain.Entities;

namespace EFCore.Fakers;

internal class TeacherFaker : Faker<Teacher> {
	public TeacherFaker() {
		RuleFor(t => t.Id, f => Guid.NewGuid());
		RuleFor(t => t.Username, f => f.Internet.UserName());
		RuleFor(t => t.FirstName, f => f.Name.FirstName());
		RuleFor(t => t.LastName, f => f.Name.LastName());
		RuleFor(t => t.PasswordHash, f => BCrypt.Net.BCrypt.HashPassword("pass"));
		RuleFor(t => t.Email, f => f.Internet.Email());
	}
}
