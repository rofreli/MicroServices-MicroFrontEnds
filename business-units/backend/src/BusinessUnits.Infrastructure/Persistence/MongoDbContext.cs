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

    public IMongoCollection<BusinessUnit> BusinessUnits =>
        _database.GetCollection<BusinessUnit>(_settings.BusinessUnitsCollection);

    private void EnsureIndexes()
    {
        var indexKeys = Builders<BusinessUnit>.IndexKeys.Ascending("Cnpj");
        var indexOptions = new CreateIndexOptions { Unique = true, Name = "idx_cnpj_unique" };
        BusinessUnits.Indexes.CreateOne(new CreateIndexModel<BusinessUnit>(indexKeys, indexOptions));
    }
}
