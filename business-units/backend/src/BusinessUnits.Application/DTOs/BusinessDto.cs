namespace BusinessUnits.Application.DTOs;

public record BusinessDto(
    string Id,
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record BusinessSummaryDto(
    string Id,
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    bool IsActive,
    DateTime CreatedAt
);
