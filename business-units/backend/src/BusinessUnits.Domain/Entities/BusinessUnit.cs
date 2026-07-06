using BusinessUnits.Domain.Events;
using BusinessUnits.Domain.Exceptions;
using BusinessUnits.Domain.ValueObjects;

namespace BusinessUnits.Domain.Entities;

public class BusinessUnit
{
    private readonly List<IDomainEvent> _domainEvents = new();
    // NOT readonly: MongoDB's MapField deserializer must be able to set this backing field.
    private List<Contact> _contacts = new();

    public string Id { get; private set; }
    public string BusinessId { get; private set; } = string.Empty;
    public string RazaoSocial { get; private set; }
    public string NomeFantasia { get; private set; }
    public Cnpj Cnpj { get; private set; }
    public Address? Address { get; private set; }
    public IReadOnlyList<Contact> Contacts => _contacts.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private BusinessUnit() { }

    public static BusinessUnit Create(string businessId, string razaoSocial, string nomeFantasia, string cnpj)
    {
        if (string.IsNullOrWhiteSpace(businessId))
            throw new DomainException("A BusinessUnit requires a Business (businessId).");

        var unit = new BusinessUnit
        {
            Id = Guid.NewGuid().ToString(),
            BusinessId = businessId,
            RazaoSocial = razaoSocial,
            NomeFantasia = nomeFantasia,
            Cnpj = new Cnpj(cnpj),
            CreatedAt = DateTime.UtcNow
        };
        unit._domainEvents.Add(new BusinessUnitCreatedEvent(unit.Id));
        return unit;
    }

    public void Update(string razaoSocial, string nomeFantasia)
    {
        RazaoSocial = razaoSocial;
        NomeFantasia = nomeFantasia;
        UpdatedAt = DateTime.UtcNow;
        _domainEvents.Add(new BusinessUnitUpdatedEvent(Id));
    }

    public void SetAddress(Address address)
    {
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }

    public Contact AddContact(string name, string email, string phone, ContactType type)
    {
        var contact = Contact.Create(name, email, phone, type);
        _contacts.Add(contact);
        UpdatedAt = DateTime.UtcNow;
        return contact;
    }

    public void RemoveContact(string contactId)
    {
        var contact = _contacts.FirstOrDefault(c => c.Id == contactId)
            ?? throw new NotFoundException("Contact", contactId);
        _contacts.Remove(contact);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
