using Application.Services;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTests.Services;
public class FileServiceTests : BaseServiceTests {
    private byte[] SampleFileContent() => [1, 2, 3, 4, 5];

    [Fact]
    public async Task UploadFileAsync_CreatesFileSuccessfully() {
        var db = CreateDbContext();
        var service = new FileService(db);
        var courseId = Guid.NewGuid();

        var file = await service.UploadFileAsync(SampleFileContent(), "test.txt", "text/plain", 5, courseId, CancellationToken.None);

        Assert.NotNull(file);
        Assert.Equal(courseId, file.CourseId);
        Assert.Equal("txt", file.FileType);
        Assert.Equal(5, file.FileSize);
        Assert.Equal("test.txt", file.FileName);
        Assert.Single(db.CourseFiles);
    }

    [Fact]
    public async Task UploadFileAsync_ThrowsOnEmptyFile() {
        var db = CreateDbContext();
        var service = new FileService(db);
        var courseId = Guid.NewGuid();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.UploadFileAsync([], "empty.txt", "text/plain", 0, courseId, CancellationToken.None)
        );
    }

    [Fact]
    public async Task GetFileAsync_ReturnsFile_WhenExists() {
        var db = CreateDbContext();
        var file = new CourseFile {
            Id = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            FileName = "sample.txt",
            FileType = "txt",
            FileSize = 5,
            FileContent = SampleFileContent(),
            CreatedAt = DateTime.UtcNow
        };
        db.CourseFiles.Add(file);
        await db.SaveChangesAsync();

        var service = new FileService(db);
        var result = await service.GetFileAsync(file.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(file.Id, result!.Id);
    }

    [Fact]
    public async Task GetFileAsync_ReturnsNull_WhenNotExists() {
        var db = CreateDbContext();
        var service = new FileService(db);

        var result = await service.GetFileAsync(Guid.NewGuid(), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteFileAsync_RemovesFileSuccessfully() {
        var db = CreateDbContext();
        var file = new CourseFile {
            Id = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            FileName = "delete.txt",
            FileType = "txt",
            FileSize = 5,
            FileContent = SampleFileContent(),
            CreatedAt = DateTime.UtcNow
        };
        db.CourseFiles.Add(file);
        await db.SaveChangesAsync();

        var service = new FileService(db);
        await service.DeleteFileAsync(file.Id, CancellationToken.None);

        Assert.Empty(db.CourseFiles);
    }

    [Fact]
    public async Task DeleteFileAsync_Throws_WhenFileNotFound() {
        var db = CreateDbContext();
        var service = new FileService(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.DeleteFileAsync(Guid.NewGuid(), CancellationToken.None)
        );
    }
}
