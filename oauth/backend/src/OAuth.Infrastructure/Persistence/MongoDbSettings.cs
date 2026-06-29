namespace OAuth.Infrastructure.Persistence;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "OAuthDb";
    public string UsersCollection { get; set; } = "users";
}
