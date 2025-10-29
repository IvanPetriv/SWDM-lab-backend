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
    public async Task<Course?> GetById(Guid id, CancellationToken ct) {
        Course? course = await dbContext.Courses
            .SingleOrDefaultAsync(e => e.Id == id, ct);

        return course;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A collection of all <see cref="Course"/> objects.</returns>
    public async Task<ICollection<Course>> GetAll(CancellationToken ct) {
        List<Course> courses = await dbContext.Courses
            .ToListAsync(ct);

        return courses;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="teacherId"></param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A collection of teacher's <see cref="Course"/> objects.</returns>
    public async Task<ICollection<Course>> GetAllOfTeacher(Guid teacherId, CancellationToken ct) {
        List<Course> courses = await dbContext.Courses
            .Where(e => e.TeacherId == teacherId)
            .ToListAsync(ct);

        return courses;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created <see cref="Course"/> object.</returns>
    public async Task<Course> Create(Course obj, CancellationToken ct) {
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
    public async Task<Course?> Update(Guid id, Course updated, CancellationToken ct) {
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
    public async Task<bool> Delete(Guid id, CancellationToken ct) {
        Course? course = await dbContext.Courses
            .SingleOrDefaultAsync(e => e.Id == id, ct);
        if (course is null)
            return false;

        dbContext.Courses.Remove(course);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }


    // --- Relational operations ---
    /// <summary>
    /// 
    /// </summary>
    /// <param name="studentId"></param>
    /// <param name="courseId"></param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns></returns>
    public async Task<Enrollment?> EnrollStudent(Guid studentId, Guid courseId, CancellationToken ct) {
        if (!await dbContext.Students.AnyAsync(s => s.Id == studentId, ct)
            || !await dbContext.Courses.AnyAsync(c => c.Id == courseId, ct)) {
            return null;
        }

        Enrollment enrollment = new() {
            StudentId = studentId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow
        };

        await dbContext.Enrollments.AddAsync(enrollment, ct);
        await dbContext.SaveChangesAsync(ct);
        return enrollment;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="studentId"></param>
    /// <param name="courseId"></param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns></returns>
    public async Task<bool> UnenrollStudent(Guid studentId, Guid courseId, CancellationToken ct) {
        Enrollment? enrollment = await dbContext.Enrollments
            .SingleOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId, ct);

        if (enrollment is null) {
            return false;
        }

        dbContext.Enrollments.Remove(enrollment);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }
}
