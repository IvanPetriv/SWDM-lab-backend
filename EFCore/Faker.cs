using Bogus;
using Domain.Entities;


namespace EFCore;
public static class FakeDataGenerator {
    // TODO: FIX
    public static List<Student> GenerateStudents(int count = 10) {
        var studentFaker = new Faker<Student>()
            .RuleFor(s => s.Id, _ => Guid.NewGuid())
            .RuleFor(s => s.Username, f => f.Internet.UserName())
            .RuleFor(s => s.PasswordHash, f => f.Internet.Password())
            .RuleFor(s => s.Email, f => f.Internet.Email())
            .RuleFor(s => s.Enrollments, _ => new List<Enrollment>());

        return studentFaker.Generate(count);
    }

    public static List<Teacher> GenerateTeachers(int count = 5) {
        var teacherFaker = new Faker<Teacher>()
            .RuleFor(t => t.Id, _ => Guid.NewGuid())
            .RuleFor(t => t.Username, f => f.Internet.UserName())
            .RuleFor(t => t.PasswordHash, f => f.Internet.Password())
            .RuleFor(t => t.Email, f => f.Internet.Email())
            .RuleFor(t => t.Courses, _ => new List<Course>());

        return teacherFaker.Generate(count);
    }

    public static List<Course> GenerateCourses(List<Teacher> teachers, int count = 10) {
        var courseFaker = new Faker<Course>()
            .RuleFor(c => c.Id, _ => Guid.NewGuid())
            .RuleFor(c => c.Name, f => f.Company.CompanyName())
            .RuleFor(c => c.Description, f => f.Lorem.Sentence(10))
            .RuleFor(c => c.Enrollments, _ => new List<Enrollment>());

        return courseFaker.Generate(count);
    }


    public static List<Enrollment> GenerateEnrollments(List<Student> students, List<Course> courses, int count = 20) {
        var enrollmentFaker = new Faker<Enrollment>()
            .RuleFor(e => e.Student, f => f.PickRandom(students))
            .RuleFor(e => e.Course, f => f.PickRandom(courses))
            .RuleFor(e => e.EnrolledAt, f => f.Date.Past(1));

        return enrollmentFaker.Generate(count);
    }
}
