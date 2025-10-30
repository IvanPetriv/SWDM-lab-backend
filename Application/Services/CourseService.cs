using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class CourseService(UniversityDbContext dbContext)
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">Course ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><see cref="Course"/> object if found, null otherwise.</returns>
    public async Task<Course?> GetById(Guid id, CancellationToken ct)
    {
        Course? course = await dbContext.Courses
            .SingleOrDefaultAsync(e => e.Id == id, ct);

        return course;
    }

    /// <summary>
    /// Gets a course by ID with all associated files (excluding file content for performance).
    /// </summary>
    /// <param name="id">Course ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><see cref="Course"/> object with files if found, null otherwise.</returns>
    public async Task<Course?> GetByIdWithFiles(Guid id, CancellationToken ct)
    {
        var course = await dbContext.Courses
            .Where(c => c.Id == id)
            .Select(c => new Course
            {
                Id = c.Id,
                TeacherId = c.TeacherId,
                Name = c.Name,
                Description = c.Description,
                Code = c.Code,
                CourseFiles = c.CourseFiles.Select(f => new CourseFile
                {
                    Id = f.Id,
                    CourseId = f.CourseId,
                    FileName = f.FileName,
                    FileType = f.FileType,
                    FileSize = f.FileSize,
                    CreatedAt = f.CreatedAt
                    // FileContent is intentionally excluded for performance
                }).ToList()
            })
            .FirstOrDefaultAsync(ct);

        return course;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A collection of all <see cref="Course"/> objects.</returns>
    public async Task<ICollection<Course>> GetAll(CancellationToken ct)
    {
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
    public async Task<ICollection<Course>> GetAllOfTeacher(Guid teacherId, CancellationToken ct)
    {
        List<Course> courses = await dbContext.Courses
            .Where(e => e.TeacherId == teacherId)
            .ToListAsync(ct);

        return courses;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="studentId"></param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A collection of student's enrolled <see cref="Course"/> objects.</returns>
    public async Task<ICollection<Course>> GetAllOfStudent(Guid studentId, CancellationToken ct)
    {
        List<Course> courses = await dbContext.Enrollments
            .Where(e => e.StudentId == studentId)
            .Select(e => e.Course)
            .ToListAsync(ct);

        return courses;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created <see cref="Course"/> object.</returns>
    public async Task<Course> Create(Course obj, CancellationToken ct)
    {
        await dbContext.Courses.AddAsync(obj, ct);
        await dbContext.SaveChangesAsync(ct);
        return obj;
    }


    /// <summary>
    /// Updates an existing course with the provided values.
    /// </summary>
    /// <param name="id">The ID of the course to update.</param>
    /// <param name="name">The new name for the course.</param>
    /// <param name="description">The new description for the course.</param>
    /// <param name="code">The new code for the course.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Updated <see cref="Course"/> object.</returns>
    public async Task<Course?> Update(Guid id, string name, string description, int code, CancellationToken ct)
    {
        Course? existing = await dbContext.Courses
            .SingleOrDefaultAsync(e => e.Id == id, ct);
        if (existing is null)
            return null;

        existing.Name = name;
        existing.Description = description;
        existing.Code = code;

        await dbContext.SaveChangesAsync(ct);
        return existing;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>true if the deletion was successful, false otherwise.</returns>
    public async Task<bool> Delete(Guid id, CancellationToken ct)
    {
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
    public async Task<Enrollment?> EnrollStudent(Guid studentId, Guid courseId, CancellationToken ct)
    {
        if (!await dbContext.Students.AnyAsync(s => s.Id == studentId, ct)
            || !await dbContext.Courses.AnyAsync(c => c.Id == courseId, ct))
        {
            return null;
        }

        Enrollment enrollment = new()
        {
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
    public async Task<bool> UnenrollStudent(Guid studentId, Guid courseId, CancellationToken ct)
    {
        Enrollment? enrollment = await dbContext.Enrollments
            .SingleOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId, ct);

        if (enrollment is null)
        {
            return false;
        }

        dbContext.Enrollments.Remove(enrollment);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>
    /// Checks if a teacher is the owner of a course.
    /// </summary>
    /// <param name="courseId">The course ID.</param>
    /// <param name="teacherId">The teacher ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if the teacher owns the course, false otherwise.</returns>
    public async Task<bool> IsTeacherOwner(Guid courseId, Guid teacherId, CancellationToken ct)
    {
        var course = await dbContext.Courses
            .Where(c => c.Id == courseId)
            .Select(c => c.TeacherId)
            .FirstOrDefaultAsync(ct);

        return course == teacherId;
    }
}
