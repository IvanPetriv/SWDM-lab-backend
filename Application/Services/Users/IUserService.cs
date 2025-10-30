using Domain.Entities;

namespace Application.Services.Users;

public interface IUserService
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct);
    Task<IEnumerable<User>> SearchByUsernameAsync(string usernamePart, CancellationToken ct);
    Task<User?> GetAsync(Guid id, CancellationToken ct);
    Task<User> CreateAsync(User user, CancellationToken ct);
    Task<User?> UpdateAsync(Guid id, User updated, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}

