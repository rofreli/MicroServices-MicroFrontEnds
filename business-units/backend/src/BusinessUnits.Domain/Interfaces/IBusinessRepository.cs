using BusinessUnits.Domain.Entities;

namespace BusinessUnits.Domain.Interfaces;

public interface IBusinessRepository
{
    Task<Business?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<Business?> GetByCnpjAsync(string cnpj, CancellationToken ct = default);
    Task<IReadOnlyList<Business>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<long> CountAsync(CancellationToken ct = default);
    Task AddAsync(Business business, CancellationToken ct = default);
    Task UpdateAsync(Business business, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
    Task<bool> ExistsByCnpjAsync(string cnpj, CancellationToken ct = default);
    Task<bool> ExistsByIdAsync(string id, CancellationToken ct = default);
}
