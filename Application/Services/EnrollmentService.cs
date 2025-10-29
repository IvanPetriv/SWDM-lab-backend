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
            .Include(e => e.Student)
            .Select(e => e.Student)
            .ToListAsync(ct);
    }

    public async Task<Enrollment> AddTeacherToCourseAsync(Guid teacherId, Guid courseId, CancellationToken ct) {
        // Assuming Course has a TeacherId property
        var course = await dbContext.Courses.FirstOrDefaultAsync(c => c.Id == courseId, ct);
        if (course == null)
            throw new InvalidOperationException("Course not found.");

        course.TeacherId = teacherId; // assign teacher
        await dbContext.SaveChangesAsync(ct);

        return new Enrollment {
            CourseId = courseId,
            UserId = Guid.Empty, // not used for teacher
            EnrolledAt = DateTime.UtcNow
        };
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
