using Domain.Entities;

namespace Application.Services.Users;

public interface IAuthUserService
{
    Task<Domain.Entities.User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<Domain.Entities.User?> GetAsync(Guid id, CancellationToken ct);
    Task<Domain.Entities.User> CreateAsync(Domain.Entities.User user, CancellationToken ct);
}

