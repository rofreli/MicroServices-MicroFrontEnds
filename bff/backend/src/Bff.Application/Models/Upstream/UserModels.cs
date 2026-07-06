namespace Bff.Application.Models.Upstream;

/// <summary>Subset of the OAuth domain <c>UserSummaryDto</c> consumed by the BFF.</summary>
public record UserSummary(
    string Id,
    string Email,
    string FullName,
    bool IsActive,
    bool IsSuperAdmin,
    DateTime CreatedAt
);

/// <summary>Subset of the OAuth domain <c>UserDto</c> (includes the permission list).</summary>
public record UserDetail(
    string Id,
    string Email,
    string FullName,
    bool IsActive,
    bool IsSuperAdmin,
    IReadOnlyList<UpstreamPermission> Permissions
);

/// <summary>Mirrors the OAuth domain <c>PermissionDto</c>.</summary>
public record UpstreamPermission(
    string BusinessId,
    string? BusinessUnitId,
    string Module,
    string? Function,
    string Role
);
