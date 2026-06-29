namespace BusinessUnits.Domain.Entities;

public enum ContactType { Primary, Secondary, Technical, Commercial }

public class Contact
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public ContactType Type { get; private set; }

    private Contact() { }

    public static Contact Create(string name, string email, string phone, ContactType type)
    {
        return new Contact
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Email = email,
            Phone = phone,
            Type = type
        };
    }
}
