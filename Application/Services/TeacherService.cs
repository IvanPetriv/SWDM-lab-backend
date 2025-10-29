using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class TeacherService(UniversityDbContext dbContext) {
    public async Task<ICollection<Teacher>> GetAll(CancellationToken ct) =>
        await dbContext.Teachers.ToListAsync(ct);
}
