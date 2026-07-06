namespace Bff.Application.Models;

/// <summary>
/// Composite view of a single Business: its details, its Business Units, and the
/// team of users holding permissions scoped to it — stitched from two domains.
/// </summary>
public record BusinessOverviewDto(
    BusinessSummaryDto Business,
    long BusinessUnitCount,
    IReadOnlyList<BusinessUnitItemDto> BusinessUnits,
    IReadOnlyList<TeamMemberDto> Team
);

public record BusinessSummaryDto(
    string Id,
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record BusinessUnitItemDto(
    string Id,
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    DateTime CreatedAt
);

/// <summary>A user who holds one or more permissions scoped to the business.</summary>
public record TeamMemberDto(
    string UserId,
    string Email,
    string FullName,
    bool IsSuperAdmin,
    IReadOnlyList<string> Roles
);
