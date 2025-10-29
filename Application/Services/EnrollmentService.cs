using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class EnrollmentService(UniversityDbContext dbContext)
{
    /// <summary>
    /// Adds a student to a course by creating an enrollment.
    /// </summary>
    public async Task<Enrollment?> AddStudentToCourse(Guid studentId, Guid courseId, CancellationToken ct)
    {
        // Check if student exists
        var studentExists = await dbContext.Students.AnyAsync(s => s.Id == studentId, ct);
        if (!studentExists)
            return null;

        // Check if course exists
        var courseExists = await dbContext.Courses.AnyAsync(c => c.Id == courseId, ct);
        if (!courseExists)
            return null;

        // Check if enrollment already exists
        var existingEnrollment = await dbContext.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId, ct);
        if (existingEnrollment != null)
            return existingEnrollment; // Already enrolled

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow,
            Grade = 0
        };

        dbContext.Enrollments.Add(enrollment);
        await dbContext.SaveChangesAsync(ct);
        return enrollment;
    }

    /// <summary>
    /// Gets all students enrolled in a specific course.
    /// </summary>
    public async Task<IEnumerable<Student>> GetCourseStudents(Guid courseId, CancellationToken ct)
    {
        var students = await dbContext.Enrollments
            .Where(e => e.CourseId == courseId)
            .Include(e => e.Student)
            .Select(e => e.Student)
            .ToListAsync(ct);

        return students;
    }

    /// <summary>
    /// Assigns a teacher to a course. Replaces the existing teacher if one is already assigned.
    /// </summary>
    public async Task<Course?> AddTeacherToCourse(Guid teacherId, Guid courseId, CancellationToken ct)
    {
        // Check if teacher exists
        var teacherExists = await dbContext.Teachers.AnyAsync(t => t.Id == teacherId, ct);
        if (!teacherExists)
            return null;

        // Get the course
        var course = await dbContext.Courses.FirstOrDefaultAsync(c => c.Id == courseId, ct);
        if (course == null)
            return null;

        // Replace the old teacher with the new one (or assign if no teacher was assigned)
        course.TeacherId = teacherId;
        await dbContext.SaveChangesAsync(ct);
        return course;
    }

    /// <summary>
    /// Removes a student from a course by deleting the enrollment.
    /// </summary>
    public async Task<bool> RemoveStudentFromCourse(Guid studentId, Guid courseId, CancellationToken ct)
    {
        var enrollment = await dbContext.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId, ct);

        if (enrollment == null)
            return false;

        dbContext.Enrollments.Remove(enrollment);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>
    /// Removes a teacher from a course by setting the TeacherId to null.
    /// </summary>
    public async Task<bool> RemoveTeacherFromCourse(Guid courseId, CancellationToken ct)
    {
        var course = await dbContext.Courses.FirstOrDefaultAsync(c => c.Id == courseId, ct);
        if (course == null)
            return false;

        course.TeacherId = null;
        await dbContext.SaveChangesAsync(ct);
        return true;
    }
}
