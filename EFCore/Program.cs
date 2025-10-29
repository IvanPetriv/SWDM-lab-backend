using EFCore;

var teachers = FakeDataGenerator.GenerateTeachers(5);
var courses = FakeDataGenerator.GenerateCourses(teachers, 10);
var students = FakeDataGenerator.GenerateStudents(50);
var enrollments = FakeDataGenerator.GenerateEnrollments(students, courses, 100);
var textMaterials = FakeDataGenerator.GenerateTextMaterials(courses, 30);
var mediaMaterials = FakeDataGenerator.GenerateMediaMaterials(courses, 30);


using UniversityDbContext context = new();
context.Teachers.AddRange(teachers);
context.Courses.AddRange(courses);
context.Students.AddRange(students);
context.Enrollments.AddRange(enrollments);
context.MediaMaterials.AddRange(mediaMaterials);
context.TextMaterials.AddRange(textMaterials);
context.SaveChanges();
