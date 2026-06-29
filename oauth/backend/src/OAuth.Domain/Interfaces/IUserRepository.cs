using OAuth.Domain.Entities;

namespace OAuth.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByExternalProviderAsync(string provider, string providerId, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<long> CountAsync(CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
}
