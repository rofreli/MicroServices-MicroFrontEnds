using OAuth.Domain.Exceptions;
using OAuth.Domain.ValueObjects;

namespace OAuth.Domain.Entities;

public class ExternalProvider
{
    public string Provider { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
}

public class User
{
    private readonly List<string> _roles = new();
    private readonly List<ExternalProvider> _externalProviders = new();

    public string Id { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? PasswordHash { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyList<string> Roles => _roles.AsReadOnly();
    public IReadOnlyList<ExternalProvider> ExternalProviders => _externalProviders.AsReadOnly();

    public string FullName => $"{FirstName} {LastName}".Trim();

    private User() { }

    public static User Create(string email, string firstName, string lastName, string? passwordHash = null)
    {
        return new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = new Email(email),
            FirstName = firstName,
            LastName = lastName,
            PasswordHash = passwordHash,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public static User CreateFromExternalProvider(
        string email, string firstName, string lastName,
        string provider, string providerId)
    {
        var user = Create(email, firstName, lastName);
        user._externalProviders.Add(new ExternalProvider { Provider = provider, ProviderId = providerId });
        return user;
    }

    public void Update(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPasswordHash(string hash)
    {
        PasswordHash = hash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive) throw new DomainException("User is already inactive.");
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive) throw new DomainException("User is already active.");
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role)) throw new DomainException("Role cannot be empty.");
        var normalized = role.Trim().ToUpperInvariant();
        if (_roles.Contains(normalized)) throw new ConflictException($"User already has role '{role}'.");
        _roles.Add(normalized);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveRole(string role)
    {
        var normalized = role.Trim().ToUpperInvariant();
        if (!_roles.Remove(normalized))
            throw new NotFoundException("Role", role);
        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkExternalProvider(string provider, string providerId)
    {
        if (_externalProviders.Any(p => p.Provider == provider && p.ProviderId == providerId)) return;
        _externalProviders.Add(new ExternalProvider { Provider = provider, ProviderId = providerId });
        UpdatedAt = DateTime.UtcNow;
    }
}
