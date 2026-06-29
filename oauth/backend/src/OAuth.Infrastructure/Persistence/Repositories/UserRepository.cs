using MongoDB.Driver;
using OAuth.Domain.Entities;
using OAuth.Domain.Interfaces;
using OAuth.Domain.ValueObjects;

namespace OAuth.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _collection;

    public UserRepository(MongoDbContext context) => _collection = context.Users;

    public async Task<User?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _collection
            .Find(Builders<User>.Filter.Eq("Email", email.ToLowerInvariant()))
            .FirstOrDefaultAsync(ct);

    public async Task<User?> GetByExternalProviderAsync(string provider, string providerId, CancellationToken ct = default)
        => await _collection.Find(
            Builders<User>.Filter.ElemMatch("externalProviders",
                Builders<ExternalProvider>.Filter.And(
                    Builders<ExternalProvider>.Filter.Eq(p => p.Provider, provider),
                    Builders<ExternalProvider>.Filter.Eq(p => p.ProviderId, providerId)
                ))).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<User>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var result = await _collection
            .Find(Builders<User>.Filter.Empty)
            .SortByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);
        return result.AsReadOnly();
    }

    public async Task<long> CountAsync(CancellationToken ct = default)
        => await _collection.CountDocumentsAsync(Builders<User>.Filter.Empty, cancellationToken: ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
        => await _collection.InsertOneAsync(user, cancellationToken: ct);

    public async Task UpdateAsync(User user, CancellationToken ct = default)
        => await _collection.ReplaceOneAsync(x => x.Id == user.Id, user, cancellationToken: ct);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalized = email.ToLowerInvariant();
        var count = await _collection.CountDocumentsAsync(
            Builders<User>.Filter.Eq("Email", normalized), cancellationToken: ct);
        return count > 0;
    }
}
