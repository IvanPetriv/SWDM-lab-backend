using Bogus;
using Domain.Entities;

namespace EFCore.Fakers;

internal class TextMaterialFaker : Faker<TextMaterial> {
	public TextMaterialFaker(List<Course> courses) {
		RuleFor(tm => tm.Id, f => Guid.NewGuid());
		RuleFor(tm => tm.CourseId, f => f.PickRandom(courses).Id);
		RuleFor(tm => tm.Title, f => f.Lorem.Sentence(3, 5));
		RuleFor(tm => tm.Description, f => f.Lorem.Paragraph());
	}
}