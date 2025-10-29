using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class TeacherService(UniversityDbContext dbContext) {
    public async Task<Teacher?> GetById(Guid id, CancellationToken ct) =>
        await dbContext.Teachers.SingleOrDefaultAsync(e => e.Id == id, ct);

    public async Task<ICollection<Teacher>> GetAll(CancellationToken ct) =>
        await dbContext.Teachers.ToListAsync(ct);

    public async Task<ICollection<Course>> GetCoursesOfTeacher(Guid teacherId, CancellationToken ct) =>
        await dbContext.Courses
            .Where(c => c.TeacherId == teacherId)
            .ToListAsync(ct);

    public async Task<Teacher> Create(Teacher obj, CancellationToken ct) {
        await dbContext.Teachers.AddAsync(obj, ct);
        await dbContext.SaveChangesAsync(ct);
        return obj;
    }

    public async Task<Teacher?> Update(Guid id, Teacher updated, CancellationToken ct) {
        Teacher? existing = await dbContext.Teachers.FindAsync([id], ct);
        if (existing is null)
            return null;

        dbContext.Entry(existing).CurrentValues.SetValues(updated);
        await dbContext.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> Delete(Guid id, CancellationToken ct) {
        Teacher? teacher = await dbContext.Teachers.FindAsync([id], ct);
        if (teacher is null)
            return false;

        dbContext.Teachers.Remove(teacher);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<Course?> AssignCourse(Guid teacherId, Guid courseId, CancellationToken ct) {
        Course? course = await dbContext.Courses.FindAsync([courseId], ct);
        Teacher? teacher = await dbContext.Teachers.FindAsync([teacherId], ct);
        if (course is null || teacher is null)
            return null;

        course.TeacherId = teacherId;
        await dbContext.SaveChangesAsync(ct);
        return course;
    }

    public async Task<bool> UnassignCourse(Guid courseId, CancellationToken ct) {
        Course? course = await dbContext.Courses.FindAsync([courseId], ct);
        if (course is null)
            return false;

        course.TeacherId = null;
        await dbContext.SaveChangesAsync(ct);
        return true;
    }
}
