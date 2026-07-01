using BusinessUnits.Domain.Entities;
using BusinessUnits.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BusinessUnits.Infrastructure.Persistence;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly MongoDbSettings _settings;

    public MongoDbContext(IOptions<MongoDbSettings> options)
    {
        MongoDbClassMaps.Register();
        _settings = options.Value;
        var client = new MongoClient(_settings.ConnectionString);
        _database = client.GetDatabase(_settings.DatabaseName);
        EnsureIndexes();
    }

    public IMongoCollection<Business> Businesses =>
        _database.GetCollection<Business>(_settings.BusinessesCollection);

    public IMongoCollection<BusinessUnit> BusinessUnits =>
        _database.GetCollection<BusinessUnit>(_settings.BusinessUnitsCollection);

    private void EnsureIndexes()
    {
        Businesses.Indexes.CreateOne(new CreateIndexModel<Business>(
            Builders<Business>.IndexKeys.Ascending("Cnpj"),
            new CreateIndexOptions { Unique = true, Name = "idx_business_cnpj_unique" }));

        BusinessUnits.Indexes.CreateOne(new CreateIndexModel<BusinessUnit>(
            Builders<BusinessUnit>.IndexKeys.Ascending("Cnpj"),
            new CreateIndexOptions { Unique = true, Name = "idx_bu_cnpj_unique" }));

        BusinessUnits.Indexes.CreateOne(new CreateIndexModel<BusinessUnit>(
            Builders<BusinessUnit>.IndexKeys.Ascending(x => x.BusinessId),
            new CreateIndexOptions { Name = "idx_bu_business_id" }));
    }
}
