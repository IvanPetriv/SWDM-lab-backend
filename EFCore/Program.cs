using EFCore;

var teachers = FakeDataGenerator.GenerateTeachers();
var courses = FakeDataGenerator.GenerateCourses(teachers);
var students = FakeDataGenerator.GenerateStudents();
var enrollments = FakeDataGenerator.GenerateEnrollments(students, courses);

using UniversityDbContext context = new();
context.Teachers.AddRange(teachers);
context.Courses.AddRange(courses);
context.Students.AddRange(students);
context.Enrollments.AddRange(enrollments);
context.SaveChanges();
