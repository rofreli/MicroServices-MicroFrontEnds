using BusinessUnits.Domain.Entities;

namespace BusinessUnits.Application.DTOs;

public record ContactDto(
    string Id,
    string Name,
    string Email,
    string Phone,
    ContactType Type
);

public record ContactInputDto(
    string Name,
    string Email,
    string Phone,
    ContactType Type
);
