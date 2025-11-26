using Bogus;
using Domain.Entities;

namespace EFCore.Fakers;

internal class CourseFaker : Faker<Course> {
	public CourseFaker(List<Teacher> teachers) {
		RuleFor(c => c.Id, f => Guid.NewGuid());
		RuleFor(c => c.TeacherId, f => f.PickRandom(teachers).Id);
		RuleFor(c => c.Name, f => f.Company.CompanyName());
		RuleFor(c => c.Description, f => f.Lorem.Sentence());
		RuleFor(c => c.Code, f => f.Random.Int(1000, 9999));
	}
}
