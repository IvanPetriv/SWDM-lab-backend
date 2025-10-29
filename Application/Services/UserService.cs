using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserService(UniversityDbContext dbContext)
{
    public async Task<User?> GetById(Guid id, CancellationToken ct)
    {
        var student = await dbContext.Students.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (student != null) return student;

        var teacher = await dbContext.Teachers.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (teacher != null) return teacher;

        var admin = await dbContext.Administrators.FirstOrDefaultAsync(u => u.Id == id, ct);
        return admin;
    }

    public async Task<IEnumerable<User>> GetAll(CancellationToken ct)
    {
        var students = await dbContext.Students.ToListAsync(ct);
        var teachers = await dbContext.Teachers.ToListAsync(ct);
        var admins = await dbContext.Administrators.ToListAsync(ct);

        var users = new List<User>();
        users.AddRange(students);
        users.AddRange(teachers);
        users.AddRange(admins);

        return users;
    }

    public async Task<User> Create(User user, CancellationToken ct)
    {
        switch (user)
        {
            case Student student:
                await dbContext.Students.AddAsync(student, ct);
                break;
            case Teacher teacher:
                await dbContext.Teachers.AddAsync(teacher, ct);
                break;
            case Administrator admin:
                await dbContext.Administrators.AddAsync(admin, ct);
                break;
            default:
                throw new ArgumentException("Unknown user type");
        }

        await dbContext.SaveChangesAsync(ct);
        return user;
    }

    public async Task<User?> Update(Guid id, string username, string firstName, string lastName, string email, CancellationToken ct)
    {
        var user = await GetById(id, ct);
        if (user == null)
            return null;

        user.Username = username;
        user.FirstName = firstName;
        user.LastName = lastName;
        user.Email = email;

        await dbContext.SaveChangesAsync(ct);
        return user;
    }

    public async Task<IEnumerable<User>> SearchByUsername(string username, CancellationToken ct)
    {
        var students = await dbContext.Students
            .Where(u => u.Username.Contains(username))
            .ToListAsync(ct);

        var teachers = await dbContext.Teachers
            .Where(u => u.Username.Contains(username))
            .ToListAsync(ct);

        var admins = await dbContext.Administrators
            .Where(u => u.Username.Contains(username))
            .ToListAsync(ct);

        var users = new List<User>();
        users.AddRange(students);
        users.AddRange(teachers);
        users.AddRange(admins);

        return users;
    }

    public async Task<IEnumerable<Student>> SearchStudentsByUsername(string username, CancellationToken ct)
    {
        return await dbContext.Students
            .Where(s => s.Username.Contains(username))
            .ToListAsync(ct);
    }

    public async Task<bool> Delete(Guid id, CancellationToken ct)
    {
        var user = await GetById(id, ct);
        if (user == null)
            return false;

        dbContext.Remove(user);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }
}
