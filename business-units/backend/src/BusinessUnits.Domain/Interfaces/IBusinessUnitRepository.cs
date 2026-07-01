using BusinessUnits.Domain.Entities;

namespace BusinessUnits.Domain.Interfaces;

public interface IBusinessUnitRepository
{
    Task<BusinessUnit?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<BusinessUnit?> GetByCnpjAsync(string cnpj, CancellationToken ct = default);
    Task<IReadOnlyList<BusinessUnit>> GetAllAsync(int page, int pageSize, string? businessId = null, CancellationToken ct = default);
    Task<long> CountAsync(string? businessId = null, CancellationToken ct = default);
    Task AddAsync(BusinessUnit businessUnit, CancellationToken ct = default);
    Task UpdateAsync(BusinessUnit businessUnit, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
    Task<bool> ExistsByCnpjAsync(string cnpj, CancellationToken ct = default);
}
