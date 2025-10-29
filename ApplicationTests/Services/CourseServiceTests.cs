using Application.Services;
using Domain.Entities;
using FluentAssertions;

namespace ApplicationTests.Services;
public class CourseServiceTests : BaseServiceTests {
    [Fact]
    public async Task CreateAsync_ShouldAddCourse() {
        // Arrange
        var db = CreateDbContext();
        var service = new CourseService(db);
        var course = new Course { Id = Guid.NewGuid(), Name = "Math" };

        // Act
        var result = await service.CreateAsync(course, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        db.Courses.Should().ContainSingle(c => c.Name == "Math");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCourse_WhenExists() {
        var db = CreateDbContext();
        var course = new Course { Id = Guid.NewGuid(), Name = "Physics" };
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var service = new CourseService(db);

        var result = await service.GetByIdAsync(course.Id, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Physics");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound() {
        var db = CreateDbContext();
        var service = new CourseService(db);

        var result = await service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCourses() {
        var db = CreateDbContext();
        db.Courses.AddRange(
            new Course { Id = Guid.NewGuid(), Name = "A" },
            new Course { Id = Guid.NewGuid(), Name = "B" });
        await db.SaveChangesAsync();

        var service = new CourseService(db);

        var result = await service.GetAllAsync(CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyCourse_WhenExists() {
        var db = CreateDbContext();
        var course = new Course { Id = Guid.NewGuid(), Name = "Old" };
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var service = new CourseService(db);
        var updated = new Course { Id = course.Id, Name = "New" };

        var result = await service.UpdateAsync(course.Id, updated, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be("New");
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenNotFound() {
        var db = CreateDbContext();
        var service = new CourseService(db);
        var updated = new Course { Id = Guid.NewGuid(), Name = "Nope" };

        var result = await service.UpdateAsync(updated.Id, updated, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCourse_WhenExists() {
        var db = CreateDbContext();
        var course = new Course { Id = Guid.NewGuid(), Name = "DeleteMe" };
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var service = new CourseService(db);

        var success = await service.DeleteAsync(course.Id, CancellationToken.None);

        success.Should().BeTrue();
        db.Courses.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound() {
        var db = CreateDbContext();
        var service = new CourseService(db);

        var success = await service.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        success.Should().BeFalse();
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnUserCourses() {
        var db = CreateDbContext();

        var user = new Student { Id = Guid.NewGuid(), Username = "user1" };
        var course = new Course { Id = Guid.NewGuid(), Name = "Course1" };
        var enrollment = new Enrollment { User = user, UserId = user.Id, Course = course, CourseId = course.Id };

        db.Students.Add(user);
        db.Courses.Add(course);
        db.Enrollments.Add(enrollment);
        await db.SaveChangesAsync();

        var service = new CourseService(db);

        var result = await service.GetByUserIdAsync(user.Id, CancellationToken.None);

        result.Should().ContainSingle(c => c.Name == "Course1");
    }

    [Fact]
    public async Task GetWithFilesAsync_ShouldIncludeMaterials() {
        var db = CreateDbContext();

        var file = new CourseFile { Id = Guid.NewGuid(), FileName = "file.pdf" };
        var media = new MediaMaterial { Id = Guid.NewGuid(), CourseFile = file };
        var course = new Course { Id = Guid.NewGuid(), Name = "CourseWithFiles", MediaMaterials = [media] };

        db.CourseFiles.Add(file);
        db.MediaMaterials.Add(media);
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var service = new CourseService(db);

        var result = await service.GetWithFilesAsync(course.Id, CancellationToken.None);

        result.Should().NotBeNull();
        result!.MediaMaterials.Should().ContainSingle();
        result.MediaMaterials.First().CourseFile!.FileName.Should().Be("file.pdf");
    }
}
