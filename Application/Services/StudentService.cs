using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;
public class StudentService(UniversityDbContext dbContext) {
    public async Task<ICollection<Student>> GetAll(CancellationToken ct) =>
        await dbContext.Students.ToListAsync(ct);
}
