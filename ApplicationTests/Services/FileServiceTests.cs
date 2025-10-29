using Application.Services;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTests.Services;
public class FileServiceTests : BaseServiceTests {
    [Fact]
    public async Task UploadFileAsync_ShouldUploadFileSuccessfully() {
        // Arrange
        var dbContext = CreateDbContext();
        var service = new FileService(dbContext);
        var course = new Course { Id = Guid.NewGuid(), Name = "Test Course" };
        dbContext.Courses.Add(course);
        await dbContext.SaveChangesAsync();

        byte[] fileBytes = [1, 2, 3];
        string fileName = "test.pdf";

        // Act
        var file = await service.UploadFileAsync(
            fileBytes, fileName, course.Id, CancellationToken.None);

        // Assert
        var stored = await dbContext.CourseFiles.FirstAsync();
        Assert.NotNull(stored);
        Assert.Equal(fileName, stored.FileName);
        Assert.Equal(fileBytes.Length, stored.FileSize);
        //Assert.Equal(course.Id, stored.CourseId);
    }

    [Fact]
    public async Task UploadFileAsync_ShouldThrow_WhenFileIsEmpty() {
        var dbContext = CreateDbContext();
        var service = new FileService(dbContext);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.UploadFileAsync([], "file.txt", Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task UploadFileAsync_ShouldThrow_WhenCourseNotFound() {
        var dbContext = CreateDbContext();
        var service = new FileService(dbContext);

        byte[] fileBytes = [1, 2, 3];
        var fakeCourseId = Guid.NewGuid();

        // Act + Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await service.UploadFileAsync(fileBytes, "test.txt", fakeCourseId, CancellationToken.None));
    }

    [Fact]
    public async Task GetFileAsync_ShouldReturnFile() {
        var dbContext = CreateDbContext();
        var service = new FileService(dbContext);

        var file = new CourseFile {
            Id = Guid.NewGuid(),
            FileName = "test.txt",
            FileType = "txt",
            FileSize = 3,
            FileContent = [1, 2, 3],
            CreatedAt = DateTime.UtcNow
        };

        dbContext.CourseFiles.Add(file);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await service.GetFileAsync(file.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(file.Id, result!.Id);
    }

    [Fact]
    public async Task GetFileAsync_ShouldReturnNull_WhenNotFound() {
        var dbContext = CreateDbContext();
        var service = new FileService(dbContext);

        var result = await service.GetFileAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteFileAsync_ShouldDeleteFileSuccessfully() {
        var dbContext = CreateDbContext();
        var service = new FileService(dbContext);

        var file = new CourseFile {
            Id = Guid.NewGuid(),
            FileName = "delete_me.txt",
            FileType = "txt",
            FileSize = 3,
            FileContent = [1, 2, 3],
            CreatedAt = DateTime.UtcNow
        };

        dbContext.CourseFiles.Add(file);
        await dbContext.SaveChangesAsync();

        // Act
        await service.DeleteFileAsync(file.Id, CancellationToken.None);

        // Assert
        Assert.False(dbContext.CourseFiles.Any());
    }

    [Fact]
    public async Task DeleteFileAsync_ShouldThrow_WhenFileNotFound() {
        var dbContext = CreateDbContext();
        var service = new FileService(dbContext);

        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await service.DeleteFileAsync(Guid.NewGuid(), CancellationToken.None));
    }
}
