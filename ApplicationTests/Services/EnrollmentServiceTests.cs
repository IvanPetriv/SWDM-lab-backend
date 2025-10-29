using Application.Services;
using Domain.Entities;
using EFCore;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTests.Services;
public class EnrollmentServiceTests : BaseServiceTests {
    private static (UniversityDbContext, EnrollmentService) CreateService() {
        var dbContext = CreateDbContext();
        var service = new EnrollmentService(dbContext);
        return (dbContext, service);
    }

    [Fact]
    public async Task AddStudentToCourseAsync_Should_Add_New_Enrollment() {
        // Arrange
        var (db, service) = CreateService();
        var student = new Student { Id = Guid.NewGuid(), Email = "s@x.com", Username = "stud1" };
        var course = new Course { Id = Guid.NewGuid(), Name = "Math" };
        db.Students.Add(student);
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        // Act
        var enrollment = await service.AddStudentToCourseAsync(student.Id, course.Id, CancellationToken.None);

        // Assert
        enrollment.Should().NotBeNull();
        enrollment.CourseId.Should().Be(course.Id);
        enrollment.UserId.Should().Be(student.Id);

        (await db.Enrollments.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task AddStudentToCourseAsync_Should_Throw_When_Already_Enrolled() {
        // Arrange
        var (db, service) = CreateService();
        var student = new Student { Id = Guid.NewGuid(), Email = "s@x.com", Username = "stud" };
        var course = new Course { Id = Guid.NewGuid(), Name = "Physics" };
        db.Students.Add(student);
        db.Courses.Add(course);
        db.Enrollments.Add(new Enrollment { UserId = student.Id, CourseId = course.Id, EnrolledAt = DateTime.UtcNow });
        await db.SaveChangesAsync();

        // Act
        var act = async () => await service.AddStudentToCourseAsync(student.Id, course.Id, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already enrolled*");
    }

    [Fact]
    public async Task GetCourseStudentsAsync_Should_Return_Only_Students() {
        var (db, service) = CreateService();

        var student1 = new Student { Id = Guid.NewGuid(), Email = "s1@x.com", Username = "s1" };
        var student2 = new Student { Id = Guid.NewGuid(), Email = "s2@x.com", Username = "s2" };
        var teacher = new Teacher { Id = Guid.NewGuid(), Email = "t@x.com", Username = "t" };
        var course = new Course { Id = Guid.NewGuid(), Name = "Programming" };

        db.AddRange(student1, student2, teacher, course);
        db.Enrollments.AddRange(
            new Enrollment { UserId = student1.Id, CourseId = course.Id, EnrolledAt = DateTime.UtcNow },
            new Enrollment { UserId = student2.Id, CourseId = course.Id, EnrolledAt = DateTime.UtcNow },
            new Enrollment { UserId = teacher.Id, CourseId = course.Id, EnrolledAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();

        // Act
        var students = await service.GetCourseStudentsAsync(course.Id, CancellationToken.None);

        // Assert
        students.Should().HaveCount(2);
        students.Select(s => s.Id).Should().Contain(new[] { student1.Id, student2.Id });
    }

    [Fact]
    public async Task GetUserCoursesAsync_Should_Return_All_Courses_For_User() {
        var (db, service) = CreateService();
        var student = new Student { Id = Guid.NewGuid(), Email = "s@x.com", Username = "s" };
        var c1 = new Course { Id = Guid.NewGuid(), Name = "History" };
        var c2 = new Course { Id = Guid.NewGuid(), Name = "Chemistry" };

        db.AddRange(student, c1, c2);
        db.Enrollments.AddRange(
            new Enrollment { UserId = student.Id, CourseId = c1.Id, EnrolledAt = DateTime.UtcNow },
            new Enrollment { UserId = student.Id, CourseId = c2.Id, EnrolledAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();

        var result = await service.GetUserCoursesAsync(student.Id, CancellationToken.None);

        result.Should().HaveCount(2);
        result.Select(c => c.Id).Should().BeEquivalentTo(new[] { c1.Id, c2.Id });
    }

    [Fact]
    public async Task RemoveEnrollmentAsync_Should_Delete_Existing_Enrollment() {
        var (db, service) = CreateService();
        var student = new Student { Id = Guid.NewGuid(), Email = "s@x.com" };
        var course = new Course { Id = Guid.NewGuid(), Name = "Art" };
        db.AddRange(student, course);
        db.Enrollments.Add(new Enrollment { UserId = student.Id, CourseId = course.Id, EnrolledAt = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var result = await service.RemoveStudentFromCourseAsync(student.Id, course.Id, CancellationToken.None);

        result.Should().BeTrue();
        (await db.Enrollments.AnyAsync()).Should().BeFalse();
    }

    [Fact]
    public async Task RemoveEnrollmentAsync_Should_ReturnFalse_When_NotFound() {
        var (db, service) = CreateService();

        var result = await service.RemoveStudentFromCourseAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        result.Should().BeFalse();
    }

}
