using BusinessUnits.Domain.Entities;
using BusinessUnits.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace BusinessUnits.Infrastructure.Configurations;

public static class MongoDbClassMaps
{
    private static bool _registered;
    private static readonly object _lock = new();

    public static void Register()
    {
        lock (_lock)
        {
            if (_registered) return;

            BsonClassMap.RegisterClassMap<Business>(map =>
            {
                map.AutoMap();
                map.MapIdMember(x => x.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
                map.MapMember(x => x.RazaoSocial);
                map.MapMember(x => x.NomeFantasia);
                map.MapMember(x => x.Cnpj).SetSerializer(new CnpjSerializer());
                map.MapMember(x => x.IsActive);
                map.MapMember(x => x.CreatedAt);
                map.MapMember(x => x.UpdatedAt);
                map.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<BusinessUnit>(map =>
            {
                map.AutoMap();
                map.MapIdMember(x => x.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
                map.MapMember(x => x.BusinessId);
                map.MapMember(x => x.RazaoSocial);
                map.MapMember(x => x.NomeFantasia);
                map.MapMember(x => x.Cnpj).SetSerializer(new CnpjSerializer());
                map.MapMember(x => x.Address);
                map.MapMember(x => x.Contacts).SetElementName("contacts");
                map.MapMember(x => x.CreatedAt);
                map.MapMember(x => x.UpdatedAt);
                map.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<Address>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<Contact>(map =>
            {
                map.AutoMap();
                map.MapIdMember(x => x.Id);
                map.SetIgnoreExtraElements(true);
            });

            _registered = true;
        }
    }
}

internal class CnpjSerializer : IBsonSerializer<Cnpj>
{
    public Type ValueType => typeof(Cnpj);

    public Cnpj Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var value = context.Reader.ReadString();
        return new Cnpj(value);
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Cnpj value)
        => context.Writer.WriteString(value.Value);

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        => Serialize(context, args, (Cnpj)value);

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => Deserialize(context, args);
}
