using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;
public class StudentService(UniversityDbContext dbContext) {
    public async Task<Student?> GetStudent(Guid id, CancellationToken ct) {
        var student = await dbContext.Students
            .SingleOrDefaultAsync(e => e.Id == id, ct);

        return student;
    }

    public async Task<Student> CreateStudent(Student obj, CancellationToken ct) {
        await dbContext.Students.AddAsync(obj, ct);
        await dbContext.SaveChangesAsync(ct);
        return obj;
    }
}
