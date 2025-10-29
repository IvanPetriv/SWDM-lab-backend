using Bogus;
using Domain.Entities;
using System.Collections.Generic;

namespace EFCore;
public static class FakeDataGenerator {
    // Generate Teachers first
    public static List<Teacher> GenerateTeachers(int count) {
        var faker = new Faker<Teacher>()
            .RuleFor(t => t.Id, f => Guid.NewGuid())
            .RuleFor(t => t.Username, f => f.Internet.UserName())
            .RuleFor(t => t.FirstName, f => f.Name.FirstName())
            .RuleFor(t => t.LastName, f => f.Name.LastName())
            .RuleFor(t => t.PasswordHash, f => f.Internet.Password())
            .RuleFor(t => t.Email, f => f.Internet.Email());

        return faker.Generate(count);
    }

    // Generate Courses referencing real Teachers
    public static List<Course> GenerateCourses(List<Teacher> teachers, int count) {
        var faker = new Faker<Course>()
            .RuleFor(c => c.Id, f => Guid.NewGuid())
            .RuleFor(c => c.Name, f => f.Company.CompanyName())
            .RuleFor(c => c.Description, f => f.Lorem.Sentence())
            .RuleFor(c => c.Code, f => f.Random.Int(1000, 9999));

        return faker.Generate(count);
    }

    // Generate Students
    public static List<Student> GenerateStudents(int count) {
        var faker = new Faker<Student>()
            .RuleFor(s => s.Id, f => Guid.NewGuid())
            .RuleFor(s => s.Username, f => f.Internet.UserName())
            .RuleFor(s => s.FirstName, f => f.Name.FirstName())
            .RuleFor(s => s.LastName, f => f.Name.LastName())
            .RuleFor(s => s.PasswordHash, f => f.Internet.Password())
            .RuleFor(s => s.Email, f => f.Internet.Email());

        return faker.Generate(count);
    }

    // Generate Enrollments referencing real Students and Courses
    public static List<Enrollment> GenerateEnrollments(List<Student> students, List<Course> courses, int count) {
        var enrollments = new List<Enrollment>();
        var existingKeys = new HashSet<(Guid, Guid)>();

        var faker = new Faker<Enrollment>()
            .RuleFor(e => e.EnrolledAt, f => f.Date.Past(1).ToUniversalTime())
            .RuleFor(e => e.Grade, f => f.Random.Int(0, 100));

        int attempts = 0;
        while (enrollments.Count < count && attempts < count * 10) {
            attempts++;
            var studentId = students[Random.Shared.Next(students.Count)].Id;
            var courseId = courses[Random.Shared.Next(courses.Count)].Id;

            if (existingKeys.Add((studentId, courseId))) {
                var e = faker.Generate();
                e.UserId = studentId;
                e.CourseId = courseId;
                enrollments.Add(e);
            }
        }

        return enrollments;
    }


    // Generate TextMaterials linked to Courses
    public static List<TextMaterial> GenerateTextMaterials(List<Course> courses, int count) {
        var faker = new Faker<TextMaterial>()
            .RuleFor(tm => tm.Id, f => Guid.NewGuid())
            .RuleFor(tm => tm.CourseId, f => f.PickRandom(courses).Id)
            .RuleFor(tm => tm.Title, f => f.Lorem.Sentence(3, 5))
            .RuleFor(tm => tm.Description, f => f.Lorem.Paragraph());

        return faker.Generate(count);
    }

    // Generate CourseFiles
    public static List<CourseFile> GenerateCourseFiles(int count) {
        var faker = new Faker<CourseFile>()
            .RuleFor(cf => cf.Id, f => Guid.NewGuid())
            .RuleFor(cf => cf.FileName, f => f.System.FileName())
            .RuleFor(cf => cf.FileType, f => f.System.FileExt())
            .RuleFor(cf => cf.FileSize, f => f.Random.Long(1024, 10_000_000))
            .RuleFor(cf => cf.FileContent, f => f.Random.Bytes(256)) // dummy content
            .RuleFor(cf => cf.CreatedAt, f => f.Date.Past(1).ToUniversalTime());

        return faker.Generate(count);
    }

    // Generate MediaMaterials referencing existing Courses and CourseFiles
    public static List<MediaMaterial> GenerateMediaMaterials(List<Course> courses, List<CourseFile> courseFiles, int count) {
        var faker = new Faker<MediaMaterial>()
            .RuleFor(mm => mm.Id, f => Guid.NewGuid())
            .RuleFor(mm => mm.Title, f => f.Lorem.Sentence(3, 5))
            .RuleFor(mm => mm.Description, f => f.Lorem.Paragraph())
            .RuleFor(mm => mm.CourseId, f => f.PickRandom(courses).Id)
            .RuleFor(mm => mm.CourseFile, f => f.PickRandom(courseFiles).Id);

        return faker.Generate(count);
    }
}
