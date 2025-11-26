using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserService(UniversityDbContext dbContext)
{
    public async Task<User?> GetById(Guid id, CancellationToken ct)
    {
		return await dbContext.Set<User>().FirstOrDefaultAsync(u => u.Id == id, ct);
	}

    public async Task<IEnumerable<User>> GetAll(CancellationToken ct)
    {
		return await dbContext.Set<User>().ToListAsync(ct);
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
		return dbContext.Set<User>().Where(u => u.Username.Contains(username));
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
        if (user is null)
            return false;

        dbContext.Remove(user);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }
}
