namespace Bff.Application.Abstractions;

/// <summary>A single permission grant carried in the caller's access token.</summary>
public record CallerPermission(
    string BusinessId,
    string? BusinessUnitId,
    string Module,
    string? Function,
    string Role
);

/// <summary>
/// Exposes the identity and authorization scope of the caller, derived from the
/// validated access token. Implemented in the API layer over the current HTTP context.
/// </summary>
public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    string? Subject { get; }
    bool IsSuperAdmin { get; }
    IReadOnlyList<CallerPermission> Permissions { get; }

    /// <summary>True if the caller is a super admin or holds any permission on the business.</summary>
    bool CanAccessBusiness(string businessId);
}
