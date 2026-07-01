using BusinessUnits.Domain.Events;
using BusinessUnits.Domain.Exceptions;
using BusinessUnits.Domain.ValueObjects;

namespace BusinessUnits.Domain.Entities;

public class Business
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public string Id { get; private set; } = string.Empty;
    public string RazaoSocial { get; private set; } = string.Empty;
    public string NomeFantasia { get; private set; } = string.Empty;
    public Cnpj Cnpj { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Business() { }

    public static Business Create(string razaoSocial, string nomeFantasia, string cnpj)
    {
        var business = new Business
        {
            Id = Guid.NewGuid().ToString(),
            RazaoSocial = razaoSocial,
            NomeFantasia = nomeFantasia,
            Cnpj = new Cnpj(cnpj),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };
        business._domainEvents.Add(new BusinessCreatedEvent(business.Id));
        return business;
    }

    public void Update(string razaoSocial, string nomeFantasia)
    {
        RazaoSocial = razaoSocial;
        NomeFantasia = nomeFantasia;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive) throw new DomainException("Business is already inactive.");
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive) throw new DomainException("Business is already active.");
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
