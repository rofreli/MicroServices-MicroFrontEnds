namespace Bff.Application.Models;

/// <summary>
/// Cross-domain landing summary: aggregates counts from the Business Units domain
/// and the OAuth (Users) domain into a single payload for the shell dashboard.
/// </summary>
public record DashboardDto(
    long BusinessCount,
    long BusinessUnitCount,
    long UserCount,
    DateTime GeneratedAt
);
