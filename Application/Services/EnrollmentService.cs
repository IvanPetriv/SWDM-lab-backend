using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;
public class EnrollmentService(UniversityDbContext dbContext) {
    public async Task<Enrollment> AddStudentToCourseAsync(Guid studentId, Guid courseId, CancellationToken ct) {
        // Check if already enrolled
        var exists = await dbContext.Enrollments
            .AnyAsync(e => e.UserId == studentId && e.CourseId == courseId, ct);
        if (exists)
            throw new InvalidOperationException("Student already enrolled in this course.");

        var enrollment = new Enrollment {
            UserId = studentId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow
        };

        await dbContext.Enrollments.AddAsync(enrollment, ct);
        await dbContext.SaveChangesAsync(ct);

        return enrollment;
    }

    public async Task<IEnumerable<Student>> GetCourseStudentsAsync(Guid courseId, CancellationToken ct) {
        return await dbContext.Enrollments
            .Where(e => e.CourseId == courseId)
            .Include(e => e.User)
            .Select(e => e.User)
            .OfType<Student>()
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Course>> GetUserCoursesAsync(Guid userId, CancellationToken ct) {
        return await dbContext.Enrollments
            .Where(e => e.UserId == userId)
            .Include(e => e.Course)
            .Select(e => e.Course)
            .ToListAsync(ct);
    }


    public async Task<Enrollment> AddTeacherToCourseAsync(Guid teacherId, Guid courseId, CancellationToken ct) {
        // Check if already enrolled
        var exists = await dbContext.Enrollments
            .AnyAsync(e => e.UserId == teacherId && e.CourseId == courseId, ct);
        if (exists)
            throw new InvalidOperationException("Student already enrolled in this course.");

        var enrollment = new Enrollment {
            UserId = teacherId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow
        };

        await dbContext.Enrollments.AddAsync(enrollment, ct);
        await dbContext.SaveChangesAsync(ct);

        return enrollment;
    }

    public async Task<bool> RemoveStudentFromCourseAsync(Guid studentId, Guid courseId, CancellationToken ct) {
        var enrollment = await dbContext.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == studentId && e.CourseId == courseId, ct);

        if (enrollment == null)
            return false;

        dbContext.Enrollments.Remove(enrollment);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }
}
