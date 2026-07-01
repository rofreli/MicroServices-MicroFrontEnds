using BusinessUnits.Domain.Entities;
using BusinessUnits.Domain.Interfaces;
using MongoDB.Driver;

namespace BusinessUnits.Infrastructure.Persistence.Repositories;

public class BusinessRepository : IBusinessRepository
{
    private readonly IMongoCollection<Business> _collection;

    public BusinessRepository(MongoDbContext context) => _collection = context.Businesses;

    public async Task<Business?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task<Business?> GetByCnpjAsync(string cnpj, CancellationToken ct = default)
    {
        var formatted = new Domain.ValueObjects.Cnpj(cnpj).Value;
        return await _collection
            .Find(Builders<Business>.Filter.Eq("Cnpj", formatted))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<Business>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var result = await _collection
            .Find(Builders<Business>.Filter.Empty)
            .SortByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);
        return result.AsReadOnly();
    }

    public async Task<long> CountAsync(CancellationToken ct = default)
        => await _collection.CountDocumentsAsync(Builders<Business>.Filter.Empty, cancellationToken: ct);

    public async Task AddAsync(Business business, CancellationToken ct = default)
        => await _collection.InsertOneAsync(business, cancellationToken: ct);

    public async Task UpdateAsync(Business business, CancellationToken ct = default)
        => await _collection.ReplaceOneAsync(x => x.Id == business.Id, business, cancellationToken: ct);

    public async Task DeleteAsync(string id, CancellationToken ct = default)
        => await _collection.DeleteOneAsync(x => x.Id == id, ct);

    public async Task<bool> ExistsByCnpjAsync(string cnpj, CancellationToken ct = default)
    {
        var formatted = new Domain.ValueObjects.Cnpj(cnpj).Value;
        var count = await _collection.CountDocumentsAsync(
            Builders<Business>.Filter.Eq("Cnpj", formatted), cancellationToken: ct);
        return count > 0;
    }

    public async Task<bool> ExistsByIdAsync(string id, CancellationToken ct = default)
    {
        var count = await _collection.CountDocumentsAsync(
            Builders<Business>.Filter.Eq(x => x.Id, id), cancellationToken: ct);
        return count > 0;
    }
}
