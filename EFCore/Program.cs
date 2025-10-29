using EFCore;

List<Domain.Entities.Teacher> teachers = FakeDataGenerator.GenerateTeachers();
List<Domain.Entities.Course> courses = FakeDataGenerator.GenerateCourses(teachers);
List<Domain.Entities.Student> students = FakeDataGenerator.GenerateStudents();
List<Domain.Entities.Enrollment> enrollments = FakeDataGenerator.GenerateEnrollments(students, courses);

using UniversityDbContext context = new();
context.Teachers.AddRange(teachers);
context.Courses.AddRange(courses);
context.Students.AddRange(students);
context.Enrollments.AddRange(enrollments);
context.SaveChanges();
