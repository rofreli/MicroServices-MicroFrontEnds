namespace OAuth.Application.DTOs;

public record UserDto(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    bool IsActive,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> ExternalProviders,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record UserSummaryDto(
    string Id,
    string Email,
    string FullName,
    bool IsActive,
    IReadOnlyList<string> Roles,
    DateTime CreatedAt
);
