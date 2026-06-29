namespace BusinessUnits.Application.DTOs;

public record BusinessUnitDto(
    string Id,
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    AddressDto? Address,
    IReadOnlyList<ContactDto> Contacts,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record BusinessUnitSummaryDto(
    string Id,
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    DateTime CreatedAt
);
