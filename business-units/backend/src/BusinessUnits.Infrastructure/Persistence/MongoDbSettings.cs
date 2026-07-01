namespace BusinessUnits.Infrastructure.Persistence;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "BusinessUnitsDb";
    public string BusinessesCollection { get; set; } = "businesses";
    public string BusinessUnitsCollection { get; set; } = "business_units";
}
