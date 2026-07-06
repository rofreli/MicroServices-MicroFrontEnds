using Bff.Application.Common;
using Bff.Application.Models.Upstream;

namespace Bff.Application.Abstractions;

/// <summary>Port over the Business Units domain API.</summary>
public interface IBusinessCatalogApi
{
    Task<long> CountBusinessesAsync(CancellationToken ct = default);

    Task<long> CountBusinessUnitsAsync(CancellationToken ct = default);

    Task<BusinessDetail?> GetBusinessAsync(string businessId, CancellationToken ct = default);

    Task<PagedResult<BusinessUnitSummary>> GetBusinessUnitsAsync(
        string businessId, int page, int pageSize, CancellationToken ct = default);
}
