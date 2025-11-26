using Bogus;
using Domain.Entities;

namespace EFCore.Fakers;

internal class MediaMaterialFaker : Faker<MediaMaterial> {
	public MediaMaterialFaker(List<Course> courses) {
		RuleFor(mm => mm.Id, f => Guid.NewGuid());
		RuleFor(mm => mm.CourseId, f => f.PickRandom(courses).Id);
		RuleFor(mm => mm.Title, f => f.Lorem.Sentence(3, 5));
		RuleFor(mm => mm.Description, f => f.Lorem.Paragraph());
		RuleFor(mm => mm.FileName, f => f.System.FileName());
		RuleFor(mm => mm.FileType, f => f.System.FileExt());
		RuleFor(mm => mm.FileSize, f => f.Random.Long(1024, 10_000_000));
		RuleFor(mm => mm.FileContent, f => f.Random.Bytes(256));
		RuleFor(mm => mm.CreatedAt, f => f.Date.Past(1).ToUniversalTime());
	}
}