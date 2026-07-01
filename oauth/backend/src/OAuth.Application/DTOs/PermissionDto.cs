namespace OAuth.Application.DTOs;

public record PermissionDto(
    string BusinessId,
    string? BusinessUnitId,
    string Module,
    string? Function,
    string Role
);
