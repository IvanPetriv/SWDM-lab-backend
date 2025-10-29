using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Users;

public class AuthUserService(UniversityDbContext dbContext) : IAuthUserService
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var student = await dbContext.Students.FirstOrDefaultAsync(u => u.Email == email, ct);
        if (student != null) return student;

        var teacher = await dbContext.Teachers.FirstOrDefaultAsync(u => u.Email == email, ct);
        if (teacher != null) return teacher;

        var admin = await dbContext.Administrators.FirstOrDefaultAsync(u => u.Email == email, ct);
        return admin;
    }

    public async Task<User?> GetAsync(Guid id, CancellationToken ct)
    {
        var student = await dbContext.Students.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (student != null) return student;

        var teacher = await dbContext.Teachers.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (teacher != null) return teacher;

        var admin = await dbContext.Administrators.FirstOrDefaultAsync(u => u.Id == id, ct);
        return admin;
    }

    public async Task<User> CreateAsync(User user, CancellationToken ct)
    {
        if (user is Student student)
            await dbContext.Students.AddAsync(student, ct);
        else if (user is Teacher teacher)
            await dbContext.Teachers.AddAsync(teacher, ct);
        else if (user is Administrator admin)
            await dbContext.Administrators.AddAsync(admin, ct);
        else
            throw new InvalidOperationException("Unknown user type");

        await dbContext.SaveChangesAsync(ct);
        return user;
    }
}