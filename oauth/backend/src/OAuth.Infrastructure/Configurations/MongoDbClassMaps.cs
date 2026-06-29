using MongoDB.Bson.Serialization;
using OAuth.Domain.Entities;
using OAuth.Domain.ValueObjects;

namespace OAuth.Infrastructure.Configurations;

public static class MongoDbClassMaps
{
    private static bool _registered;
    private static readonly object _lock = new();

    public static void Register()
    {
        lock (_lock)
        {
            if (_registered) return;

            BsonClassMap.RegisterClassMap<User>(map =>
            {
                map.AutoMap();
                map.MapIdMember(x => x.Id);
                map.MapMember(x => x.Email).SetSerializer(new EmailSerializer());
                map.MapMember(x => x.FirstName);
                map.MapMember(x => x.LastName);
                map.MapMember(x => x.PasswordHash);
                map.MapMember(x => x.IsActive);
                map.MapField("_roles").SetElementName("roles");
                map.MapField("_externalProviders").SetElementName("externalProviders");
                map.MapMember(x => x.CreatedAt);
                map.MapMember(x => x.UpdatedAt);
                map.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<ExternalProvider>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });

            _registered = true;
        }
    }
}

internal class EmailSerializer : MongoDB.Bson.Serialization.IBsonSerializer<Email>
{
    public Type ValueType => typeof(Email);

    public Email Deserialize(MongoDB.Bson.Serialization.BsonDeserializationContext context, MongoDB.Bson.Serialization.BsonDeserializationArgs args)
        => new(context.Reader.ReadString());

    public void Serialize(MongoDB.Bson.Serialization.BsonSerializationContext context, MongoDB.Bson.Serialization.BsonSerializationArgs args, Email value)
        => context.Writer.WriteString(value.Value);

    public void Serialize(MongoDB.Bson.Serialization.BsonSerializationContext context, MongoDB.Bson.Serialization.BsonSerializationArgs args, object value)
        => Serialize(context, args, (Email)value);

    object MongoDB.Bson.Serialization.IBsonSerializer.Deserialize(MongoDB.Bson.Serialization.BsonDeserializationContext context, MongoDB.Bson.Serialization.BsonDeserializationArgs args)
        => Deserialize(context, args);
}
