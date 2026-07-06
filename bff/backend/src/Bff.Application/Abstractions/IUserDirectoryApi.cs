using Bff.Application.Models.Upstream;

namespace Bff.Application.Abstractions;

/// <summary>Port over the OAuth (Users) domain API.</summary>
public interface IUserDirectoryApi
{
    Task<long> CountUsersAsync(CancellationToken ct = default);

    /// <summary>
    /// Returns the users holding at least one permission scoped to <paramref name="businessId"/>,
    /// together with the (distinct) roles they hold on it. The number of users scanned is bounded
    /// by <paramref name="maxUsers"/> to keep the fan-out predictable.
    /// </summary>
    Task<IReadOnlyList<UserDetail>> GetUsersWithBusinessPermissionAsync(
        string businessId, int maxUsers, CancellationToken ct = default);
}
