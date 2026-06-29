using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OAuth.Domain.Entities;
using OAuth.Infrastructure.Configurations;

namespace OAuth.Infrastructure.Persistence;

public class MongoDbContext
{
    public IMongoDatabase Database { get; }
    private readonly MongoDbSettings _settings;

    public MongoDbContext(IOptions<MongoDbSettings> options)
    {
        MongoDbClassMaps.Register();
        _settings = options.Value;
        var client = new MongoClient(_settings.ConnectionString);
        Database = client.GetDatabase(_settings.DatabaseName);
        EnsureIndexes();
    }

    public IMongoCollection<User> Users =>
        Database.GetCollection<User>(_settings.UsersCollection);

    private void EnsureIndexes()
    {
        var emailIndex = Builders<User>.IndexKeys.Ascending("Email");
        Users.Indexes.CreateOne(new CreateIndexModel<User>(
            emailIndex, new CreateIndexOptions { Unique = true, Name = "idx_email_unique" }));
    }
}
