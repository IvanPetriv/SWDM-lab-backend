using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class PostService(UniversityDbContext dbContext)
{
    /// <summary>
    /// Gets a post by ID.
    /// </summary>
    /// <param name="id">Post ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Post object if found, null otherwise.</returns>
    public async Task<Post?> GetById(Guid id, CancellationToken ct)
    {
        return await dbContext.Posts
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    /// <summary>
    /// Gets all posts for a specific course.
    /// </summary>
    /// <param name="courseId">Course ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Collection of posts for the course.</returns>
    public async Task<ICollection<Post>> GetByCourseId(Guid courseId, CancellationToken ct)
    {
        return await dbContext.Posts
            .Where(p => p.CourseId == courseId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Gets all posts created by a specific user.
    /// </summary>
    /// <param name="authorId">Author ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Collection of posts by the author.</returns>
    public async Task<ICollection<Post>> GetByAuthorId(Guid authorId, CancellationToken ct)
    {
        return await dbContext.Posts
            .Where(p => p.AuthorId == authorId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Creates a new post.
    /// </summary>
    /// <param name="post">Post object to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created post object.</returns>
    public async Task<Post> Create(Post post, CancellationToken ct)
    {
        post.Id = Guid.NewGuid();
        post.CreatedAt = DateTime.UtcNow;

        await dbContext.Posts.AddAsync(post, ct);
        await dbContext.SaveChangesAsync(ct);

        return post;
    }

    /// <summary>
    /// Updates an existing post.
    /// </summary>
    /// <param name="id">Post ID.</param>
    /// <param name="title">New title (optional).</param>
    /// <param name="textContent">New text content (optional).</param>
    /// <param name="imageData">New image data (optional).</param>
    /// <param name="imageContentType">New image content type (optional).</param>
    /// <param name="removeImage">Whether to remove the image.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Updated post object if found, null otherwise.</returns>
    public async Task<Post?> Update(
        Guid id,
        string? title,
        string? textContent,
        byte[]? imageData,
        string? imageContentType,
        bool removeImage,
        CancellationToken ct)
    {
        var post = await dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (post is null)
            return null;

        if (title is not null)
            post.Title = title;

        if (textContent is not null)
            post.TextContent = textContent;

        if (removeImage)
        {
            post.ImageData = null;
            post.ImageContentType = null;
        }
        else if (imageData is not null)
        {
            post.ImageData = imageData;
            post.ImageContentType = imageContentType;
        }

        post.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(ct);
        return post;
    }

    /// <summary>
    /// Deletes a post.
    /// </summary>
    /// <param name="id">Post ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if deletion was successful, false otherwise.</returns>
    public async Task<bool> Delete(Guid id, CancellationToken ct)
    {
        var post = await dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (post is null)
            return false;

        dbContext.Posts.Remove(post);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>
    /// Checks if a course exists.
    /// </summary>
    /// <param name="courseId">Course ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if course exists, false otherwise.</returns>
    public async Task<bool> CourseExists(Guid courseId, CancellationToken ct)
    {
        return await dbContext.Courses.AnyAsync(c => c.Id == courseId, ct);
    }

    /// <summary>
    /// Checks if a user is enrolled in a course (for students) or is the teacher of the course.
    /// </summary>
    /// <param name="courseId">Course ID.</param>
    /// <param name="userId">User ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if user has access to the course, false otherwise.</returns>
    public async Task<bool> UserHasAccessToCourse(Guid courseId, Guid userId, CancellationToken ct)
    {
        // Check if user is the teacher
        var isTeacher = await dbContext.Courses
            .AnyAsync(c => c.Id == courseId && c.TeacherId == userId, ct);

        if (isTeacher)
            return true;

        // Check if user is enrolled as a student
        var isEnrolled = await dbContext.Enrollments
            .AnyAsync(e => e.CourseId == courseId && e.StudentId == userId, ct);

        return isEnrolled;
    }

    /// <summary>
    /// Gets the image data for a post.
    /// </summary>
    /// <param name="id">Post ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Tuple containing image data and content type if found, null otherwise.</returns>
    public async Task<(byte[] imageData, string contentType)?> GetPostImage(Guid id, CancellationToken ct)
    {
        var post = await dbContext.Posts
            .Where(p => p.Id == id)
            .Select(p => new { p.ImageData, p.ImageContentType })
            .FirstOrDefaultAsync(ct);

        if (post?.ImageData is null || post.ImageContentType is null)
            return null;

        return (post.ImageData, post.ImageContentType);
    }
}
