using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;
public class CourseService(UniversityDbContext dbContext) {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">Course ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><see cref="Course"/> object if found, null otherwise.</returns>
    public async Task<Course?> GetByIdAsync(Guid id, CancellationToken ct) {
        Course? course = await dbContext.Courses
            .SingleOrDefaultAsync(e => e.Id == id, ct);

        return course;
    }


    public async Task<Course?> GetWithFilesAsync(Guid id, CancellationToken ct) {
        return await dbContext.Courses
            .Include(c => c.MediaMaterials)
                .ThenInclude(m => m.CourseFile)
            .Include(c => c.TextMaterials)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A collection of all <see cref="Course"/> objects.</returns>
    public async Task<ICollection<Course>> GetAllAsync(CancellationToken ct) {
        List<Course> courses = await dbContext.Courses
            .ToListAsync(ct);

        return courses;
    }


    public async Task<IEnumerable<Course>> GetByUserIdAsync(Guid userId, CancellationToken ct) {
        // Assuming a many-to-many: Users <-> Courses
        var enrollments = await dbContext.Enrollments
            .Where(e => e.UserId == userId)
            .Include(e => e.Course)
            .Select(e => e.Course)
            .ToListAsync(ct);

        return enrollments;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created <see cref="Course"/> object.</returns>
    public async Task<Course> CreateAsync(Course obj, CancellationToken ct) {
        await dbContext.Courses.AddAsync(obj, ct);
        await dbContext.SaveChangesAsync(ct);
        return obj;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="updated"></param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Updated <see cref="Course"/> object.</returns>
    public async Task<Course?> UpdateAsync(Guid id, Course updated, CancellationToken ct) {
        Course? existing = await dbContext.Courses
            .SingleOrDefaultAsync(e => e.Id == id, ct);
        if (existing is null)
            return null;

        dbContext.Entry(existing).CurrentValues.SetValues(updated);
        await dbContext.SaveChangesAsync(ct);
        return existing;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>true if the deletion was successful, false otherwise.</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct) {
        Course? course = await dbContext.Courses
            .SingleOrDefaultAsync(e => e.Id == id, ct);
        if (course is null)
            return false;

        dbContext.Courses.Remove(course);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }
}
