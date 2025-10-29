using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;
public class FileService(UniversityDbContext dbContext) {
    public async Task<CourseFile> UploadFileAsync(
        byte[] fileContent,
        string fileName,
        string fileType,
        long fileSize,
        Guid courseId,
        CancellationToken ct
        ) {
        if (fileContent == null || fileContent.Length == 0) {
            throw new ArgumentException("File is empty", nameof(fileContent));
        }

        var courseFile = new CourseFile {
            Id = Guid.NewGuid(),
            FileName = fileName,
            FileType = Path.GetExtension(fileName)?.TrimStart('.') ?? "",
            FileSize = fileContent.Length,
            FileContent = fileContent,
            CreatedAt = DateTime.UtcNow
        };

        await dbContext.CourseFiles.AddAsync(courseFile, ct);
        await dbContext.SaveChangesAsync(ct);

        return courseFile;
    }

    public async Task<CourseFile?> GetFileAsync(Guid id, CancellationToken ct) {
        return await dbContext.CourseFiles.FindAsync([id], ct);
    }

    public async Task DeleteFileAsync(Guid id, CancellationToken ct) {
        var file = await dbContext.CourseFiles.FindAsync([id], ct);
        if (file is null) {
            throw new KeyNotFoundException("File not found");
        }

        dbContext.CourseFiles.Remove(file);
        await dbContext.SaveChangesAsync(ct);
    }
}

