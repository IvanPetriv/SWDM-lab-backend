using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class AdministratorService(UniversityDbContext dbContext) {
    public async Task<Administrator?> GetById(Guid id, CancellationToken ct) =>
        await dbContext.Administrators.SingleOrDefaultAsync(a => a.Id == id, ct);

    public async Task<ICollection<Administrator>> GetAll(CancellationToken ct) =>
        await dbContext.Administrators.ToListAsync(ct);

    public async Task<Administrator> Create(Administrator obj, CancellationToken ct) {
        await dbContext.Administrators.AddAsync(obj, ct);
        await dbContext.SaveChangesAsync(ct);
        return obj;
    }

    public async Task<Administrator?> Update(Guid id, Administrator updated, CancellationToken ct) {
        Administrator? existing = await dbContext.Administrators.FindAsync([id], ct);
        if (existing is null)
            return null;

        dbContext.Entry(existing).CurrentValues.SetValues(updated);
        await dbContext.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> Delete(Guid id, CancellationToken ct) {
        Administrator? admin = await dbContext.Administrators.FindAsync([id], ct);
        if (admin is null)
            return false;

        dbContext.Administrators.Remove(admin);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    // User management actions
    public async Task<ICollection<Student>> GetAllStudents(CancellationToken ct) =>
        await dbContext.Students.ToListAsync(ct);

    public async Task<ICollection<Teacher>> GetAllTeachers(CancellationToken ct) =>
        await dbContext.Teachers.ToListAsync(ct);

    public async Task<bool> DeleteStudent(Guid id, CancellationToken ct) {
        Student? student = await dbContext.Students.FindAsync([id], ct);
        if (student is null)
            return false;

        dbContext.Students.Remove(student);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteTeacher(Guid id, CancellationToken ct) {
        Teacher? teacher = await dbContext.Teachers.FindAsync([id], ct);
        if (teacher is null)
            return false;

        dbContext.Teachers.Remove(teacher);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    // Course management
    public async Task<ICollection<Course>> GetAllCourses(CancellationToken ct) =>
        await dbContext.Courses.ToListAsync(ct);

    public async Task<bool> DeleteCourse(Guid id, CancellationToken ct) {
        Course? course = await dbContext.Courses.FindAsync([id], ct);
        if (course is null)
            return false;

        dbContext.Courses.Remove(course);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }
}
