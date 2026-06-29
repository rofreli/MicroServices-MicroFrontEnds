namespace BusinessUnits.Domain.Entities;

public class Address
{
    public string Street { get; private set; }
    public string Number { get; private set; }
    public string? Complement { get; private set; }
    public string District { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string ZipCode { get; private set; }
    public string Country { get; private set; }

    private Address() { }

    public static Address Create(
        string street, string number, string district,
        string city, string state, string zipCode,
        string country = "Brasil", string? complement = null)
    {
        return new Address
        {
            Street = street,
            Number = number,
            Complement = complement,
            District = district,
            City = city,
            State = state,
            ZipCode = zipCode,
            Country = country
        };
    }
}
