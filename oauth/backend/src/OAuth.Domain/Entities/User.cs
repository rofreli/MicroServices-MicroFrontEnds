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
    private readonly List<Permission> _permissions = new();
    private readonly List<ExternalProvider> _externalProviders = new();

    public string Id { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? PasswordHash { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsSuperAdmin { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyList<Permission> Permissions => _permissions.AsReadOnly();
    public IReadOnlyList<ExternalProvider> ExternalProviders => _externalProviders.AsReadOnly();

    public string FullName => $"{FirstName} {LastName}".Trim();

    private User() { }

    public static User Create(
        string email,
        string firstName,
        string lastName,
        string? passwordHash = null,
        bool isSuperAdmin = false)
    {
        return new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = new Email(email),
            FirstName = firstName,
            LastName = lastName,
            PasswordHash = passwordHash,
            IsSuperAdmin = isSuperAdmin,
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

    public void AddPermission(Permission permission)
    {
        // Replace existing permission for the same scope if any
        var existing = _permissions.FirstOrDefault(p =>
            p.Matches(permission.BusinessId, permission.BusinessUnitId,
                      permission.Module, permission.Function));
        if (existing is not null)
            _permissions.Remove(existing);

        _permissions.Add(permission);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemovePermission(
        string businessId, string? businessUnitId, string module, string? function)
    {
        var permission = _permissions.FirstOrDefault(p =>
            p.Matches(businessId, businessUnitId, module, function))
            ?? throw new NotFoundException("Permission",
               $"{businessId}/{businessUnitId ?? "*"}/{module}/{function ?? "*"}");

        _permissions.Remove(permission);
        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkExternalProvider(string provider, string providerId)
    {
        if (_externalProviders.Any(p => p.Provider == provider && p.ProviderId == providerId)) return;
        _externalProviders.Add(new ExternalProvider { Provider = provider, ProviderId = providerId });
        UpdatedAt = DateTime.UtcNow;
    }
}
