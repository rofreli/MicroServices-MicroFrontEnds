using BusinessUnits.Domain.Entities;
using BusinessUnits.Domain.Interfaces;
using MongoDB.Driver;

namespace BusinessUnits.Infrastructure.Persistence.Repositories;

public class BusinessUnitRepository : IBusinessUnitRepository
{
    private readonly IMongoCollection<BusinessUnit> _collection;

    public BusinessUnitRepository(MongoDbContext context)
        => _collection = context.BusinessUnits;

    public async Task<BusinessUnit?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task<BusinessUnit?> GetByCnpjAsync(string cnpj, CancellationToken ct = default)
        => await _collection.Find(x => x.Cnpj == new Domain.ValueObjects.Cnpj(cnpj)).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<BusinessUnit>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var result = await _collection
            .Find(Builders<BusinessUnit>.Filter.Empty)
            .SortByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);
        return result.AsReadOnly();
    }

    public async Task<long> CountAsync(CancellationToken ct = default)
        => await _collection.CountDocumentsAsync(Builders<BusinessUnit>.Filter.Empty, cancellationToken: ct);

    public async Task AddAsync(BusinessUnit businessUnit, CancellationToken ct = default)
        => await _collection.InsertOneAsync(businessUnit, cancellationToken: ct);

    public async Task UpdateAsync(BusinessUnit businessUnit, CancellationToken ct = default)
        => await _collection.ReplaceOneAsync(x => x.Id == businessUnit.Id, businessUnit, cancellationToken: ct);

    public async Task DeleteAsync(string id, CancellationToken ct = default)
        => await _collection.DeleteOneAsync(x => x.Id == id, ct);

    public async Task<bool> ExistsByCnpjAsync(string cnpj, CancellationToken ct = default)
    {
        var formatted = new Domain.ValueObjects.Cnpj(cnpj).Value;
        var count = await _collection.CountDocumentsAsync(
            Builders<BusinessUnit>.Filter.Eq("Cnpj", formatted), cancellationToken: ct);
        return count > 0;
    }
}
