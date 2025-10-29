using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;
public class StudentService(UniversityDbContext dbContext) {
    public async Task<Student?> GetById(Guid id, CancellationToken ct) {
        Student? student = await dbContext.Students
            .SingleOrDefaultAsync(e => e.Id == id, ct);

        return student;
    }

    public async Task<ICollection<Student>> GetAll(CancellationToken ct) =>
        await dbContext.Students.ToListAsync(ct);


    public async Task<ICollection<Student>> GetStudentsInCourse(Guid courseId, CancellationToken ct) =>
        await dbContext.Enrollments
            .Where(e => e.CourseId == courseId)
            .Include(e => e.Student)
            .Select(e => e.Student)
            .ToListAsync(ct);

    public async Task<Student> CreateStudent(Student obj, CancellationToken ct) {
        await dbContext.Students.AddAsync(obj, ct);
        await dbContext.SaveChangesAsync(ct);
        return obj;
    }

    public async Task<Student?> Update(Guid id, Student updated, CancellationToken ct) {
        Student? existing = await dbContext.Students.FindAsync([id], ct);
        if (existing is null)
            return null;

        dbContext.Entry(existing).CurrentValues.SetValues(updated);
        await dbContext.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> Delete(Guid id, CancellationToken ct) {
        Student? student = await dbContext.Students.FindAsync([id], ct);
        if (student is null)
            return false;

        dbContext.Students.Remove(student);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }
}
