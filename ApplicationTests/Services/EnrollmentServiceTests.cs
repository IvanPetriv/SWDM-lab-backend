using Application.Services;
using Domain.Entities;
using EFCore;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTests.Services;
public class EnrollmentServiceTests : BaseServiceTests {
    private Student CreateStudent(Guid? id = null) {
        return new Student {
            Id = id ?? Guid.NewGuid(),
            Username = "student",
            FirstName = "Jane",
            LastName = "Doe",
            Email = "student@example.com",
            PasswordHash = "hash",
            RefreshTokens = new List<RefreshToken>()
        };
    }

    private Teacher CreateTeacher(Guid? id = null) {
        return new Teacher {
            Id = id ?? Guid.NewGuid(),
            Username = "teacher",
            FirstName = "John",
            LastName = "Smith",
            Email = "teacher@example.com",
            PasswordHash = "hash",
            RefreshTokens = new List<RefreshToken>()
        };
    }

    private Course CreateCourse(Guid? id = null, Guid? teacherId = null) {
        return new Course {
            Id = id ?? Guid.NewGuid(),
            Name = "Test Course",
            Description = "Description",
            Code = 101,
            TeacherId = teacherId
        };
    }

    [Fact]
    public async Task AddStudentToCourse_AddsEnrollment() {
        var db = CreateDbContext();
        var student = CreateStudent();
        var course = CreateCourse();
        db.Students.Add(student);
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var service = new EnrollmentService(db);
        var enrollment = await service.AddStudentToCourse(student.Id, course.Id, CancellationToken.None);

        Assert.NotNull(enrollment);
        Assert.Equal(student.Id, enrollment!.StudentId);
        Assert.Equal(course.Id, enrollment.CourseId);
    }

    [Fact]
    public async Task AddStudentToCourse_ReturnsExistingEnrollment() {
        var db = CreateDbContext();
        var student = CreateStudent();
        var course = CreateCourse();
        var enrollment = new Enrollment { StudentId = student.Id, CourseId = course.Id, EnrolledAt = DateTime.UtcNow };
        db.Students.Add(student);
        db.Courses.Add(course);
        db.Enrollments.Add(enrollment);
        await db.SaveChangesAsync();

        var service = new EnrollmentService(db);
        var result = await service.AddStudentToCourse(student.Id, course.Id, CancellationToken.None);

        Assert.Equal(enrollment.StudentId, result!.StudentId);
        Assert.Equal(enrollment.CourseId, result!.CourseId);
    }

    [Fact]
    public async Task AddStudentToCourse_ReturnsNull_WhenInvalidStudentOrCourse() {
        var db = CreateDbContext();
        var service = new EnrollmentService(db);

        var enrollment1 = await service.AddStudentToCourse(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);
        Assert.Null(enrollment1);

        var student = CreateStudent();
        db.Students.Add(student);
        await db.SaveChangesAsync();

        var enrollment2 = await service.AddStudentToCourse(student.Id, Guid.NewGuid(), CancellationToken.None);
        Assert.Null(enrollment2);
    }

    [Fact]
    public async Task GetCourseStudents_ReturnsEnrolledStudents() {
        var db = CreateDbContext();
        var student1 = CreateStudent();
        var student2 = CreateStudent();
        var course = CreateCourse();
        db.Students.AddRange(student1, student2);
        db.Courses.Add(course);
        db.Enrollments.AddRange(
            new Enrollment { StudentId = student1.Id, CourseId = course.Id },
            new Enrollment { StudentId = student2.Id, CourseId = course.Id }
        );
        await db.SaveChangesAsync();

        var service = new EnrollmentService(db);
        var students = await service.GetCourseStudents(course.Id, CancellationToken.None);

        Assert.Equal(2, students.Count());
        Assert.Contains(students, s => s.Id == student1.Id);
        Assert.Contains(students, s => s.Id == student2.Id);
    }

    [Fact]
    public async Task AddTeacherToCourse_AssignsTeacher() {
        var db = CreateDbContext();
        var teacher = CreateTeacher();
        var course = CreateCourse();
        db.Teachers.Add(teacher);
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var service = new EnrollmentService(db);
        var result = await service.AddTeacherToCourse(teacher.Id, course.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(teacher.Id, result!.TeacherId);
    }

    [Fact]
    public async Task AddTeacherToCourse_ReturnsNull_WhenInvalidTeacherOrCourse() {
        var db = CreateDbContext();
        var service = new EnrollmentService(db);

        var result1 = await service.AddTeacherToCourse(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);
        Assert.Null(result1);

        var teacher = CreateTeacher();
        db.Teachers.Add(teacher);
        await db.SaveChangesAsync();

        var result2 = await service.AddTeacherToCourse(teacher.Id, Guid.NewGuid(), CancellationToken.None);
        Assert.Null(result2);
    }

    [Fact]
    public async Task RemoveStudentFromCourse_RemovesEnrollment() {
        var db = CreateDbContext();
        var student = CreateStudent();
        var course = CreateCourse();
        var enrollment = new Enrollment { StudentId = student.Id, CourseId = course.Id };
        db.Students.Add(student);
        db.Courses.Add(course);
        db.Enrollments.Add(enrollment);
        await db.SaveChangesAsync();

        var service = new EnrollmentService(db);
        var result = await service.RemoveStudentFromCourse(student.Id, course.Id, CancellationToken.None);

        Assert.True(result);
        Assert.Empty(db.Enrollments);
    }

    [Fact]
    public async Task RemoveStudentFromCourse_ReturnsFalse_WhenNotEnrolled() {
        var db = CreateDbContext();
        var service = new EnrollmentService(db);

        var result = await service.RemoveStudentFromCourse(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveTeacherFromCourse_RemovesTeacher() {
        var db = CreateDbContext();
        var teacher = CreateTeacher();
        var course = CreateCourse(teacherId: teacher.Id);
        db.Teachers.Add(teacher);
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var service = new EnrollmentService(db);
        var result = await service.RemoveTeacherFromCourse(course.Id, CancellationToken.None);

        Assert.True(result);
        Assert.Null(db.Courses.First().TeacherId);
    }

    [Fact]
    public async Task RemoveTeacherFromCourse_ReturnsFalse_WhenCourseNotFound() {
        var db = CreateDbContext();
        var service = new EnrollmentService(db);

        var result = await service.RemoveTeacherFromCourse(Guid.NewGuid(), CancellationToken.None);
        Assert.False(result);
    }

}
