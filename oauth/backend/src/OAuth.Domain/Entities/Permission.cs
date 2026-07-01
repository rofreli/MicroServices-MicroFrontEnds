using OAuth.Domain.Constants;
using OAuth.Domain.Exceptions;

namespace OAuth.Domain.Entities;

public class Permission
{
    public string BusinessId { get; private set; } = string.Empty;
    public string? BusinessUnitId { get; private set; }
    public string Module { get; private set; } = string.Empty;
    public string? Function { get; private set; }
    public string Role { get; private set; } = string.Empty;

    private Permission() { }

    public static Permission Create(
        string businessId,
        string? businessUnitId,
        string module,
        string? function,
        string role)
    {
        if (string.IsNullOrWhiteSpace(businessId))
            throw new DomainException("BusinessId is required for a permission.");
        if (!OAuthModules.All.Contains(module))
            throw new DomainException($"Unknown module '{module}'. Valid: {string.Join(", ", OAuthModules.All)}");
        if (function is not null && !OAuthFunctions.All.Contains(function))
            throw new DomainException($"Unknown function '{function}'. Valid: {string.Join(", ", OAuthFunctions.All)}");
        if (!OAuthRoles.All.Contains(role))
            throw new DomainException($"Unknown role '{role}'. Valid: {string.Join(", ", OAuthRoles.All)}");

        return new Permission
        {
            BusinessId = businessId,
            BusinessUnitId = string.IsNullOrWhiteSpace(businessUnitId) ? null : businessUnitId,
            Module = module,
            Function = function,
            Role = role,
        };
    }

    public bool Matches(string businessId, string? businessUnitId, string module, string? function)
        => BusinessId == businessId
        && BusinessUnitId == (string.IsNullOrWhiteSpace(businessUnitId) ? null : businessUnitId)
        && Module == module
        && Function == function;
}
