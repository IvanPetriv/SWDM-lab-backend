using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Users;

public class UserService(UniversityDbContext dbContext) : IUserService
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

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct) {
        var student = await dbContext.Students.FirstOrDefaultAsync(u => u.Username == username, ct);
        if (student != null)
            return student;

        var teacher = await dbContext.Teachers.FirstOrDefaultAsync(u => u.Username == username, ct);
        if (teacher != null)
            return teacher;

        var admin = await dbContext.Administrators.FirstOrDefaultAsync(u => u.Username == username, ct);
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


    public async Task<User?> UpdateAsync(Guid id, User updated, CancellationToken ct) {
        User? entity = null;

        var student = await dbContext.Students.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (student != null)
            entity = student;
        else {
            var teacher = await dbContext.Teachers.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (teacher != null)
                entity = teacher;
            else {
                var admin = await dbContext.Administrators.FirstOrDefaultAsync(u => u.Id == id, ct);
                if (admin != null)
                    entity = admin;
            }
        }

        if (entity == null)
            return null;

        dbContext.Entry(entity).CurrentValues.SetValues(updated);
        await dbContext.SaveChangesAsync(ct);

        return entity;
    }



    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct) {
        User? entity = null;
        var student = await dbContext.Students.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (student != null)
            entity = student;
        else {
            var teacher = await dbContext.Teachers.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (teacher != null)
                entity = teacher;
            else {
                var admin = await dbContext.Administrators.FirstOrDefaultAsync(u => u.Id == id, ct);
                if (admin != null)
                    entity = admin;
            }
        }

        if (entity == null)
            return false;

        switch (entity) {
            case Student student1:
                dbContext.Students.Remove(student1);
                break;
            case Teacher teacher:
                dbContext.Teachers.Remove(teacher);
                break;
            case Administrator admin:
                dbContext.Administrators.Remove(admin);
                break;
        }

        await dbContext.SaveChangesAsync(ct);
        return true;
    }
}