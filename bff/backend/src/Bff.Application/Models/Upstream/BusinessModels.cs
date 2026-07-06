namespace Bff.Application.Models.Upstream;

/// <summary>Subset of the Business Units domain <c>BusinessDto</c> consumed by the BFF.</summary>
public record BusinessDetail(
    string Id,
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

/// <summary>Subset of the domain <c>BusinessUnitSummaryDto</c> consumed by the BFF.</summary>
public record BusinessUnitSummary(
    string Id,
    string BusinessId,
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    DateTime CreatedAt
);
