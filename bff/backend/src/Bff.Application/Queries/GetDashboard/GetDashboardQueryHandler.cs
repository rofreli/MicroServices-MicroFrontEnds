using Bff.Application.Abstractions;
using Bff.Application.Models;
using MediatR;

namespace Bff.Application.Queries.GetDashboard;

/// <summary>
/// Fans out to both domains in parallel and composes their totals into a single
/// dashboard payload. A failure in any upstream call surfaces as an
/// <see cref="Common.UpstreamException"/> (mapped to 502 by the API).
/// </summary>
public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IBusinessCatalogApi _businessCatalog;
    private readonly IUserDirectoryApi _userDirectory;

    public GetDashboardQueryHandler(
        IBusinessCatalogApi businessCatalog,
        IUserDirectoryApi userDirectory)
        => (_businessCatalog, _userDirectory) = (businessCatalog, userDirectory);

    public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken ct)
    {
        var businessesTask = _businessCatalog.CountBusinessesAsync(ct);
        var businessUnitsTask = _businessCatalog.CountBusinessUnitsAsync(ct);
        var usersTask = _userDirectory.CountUsersAsync(ct);

        await Task.WhenAll(businessesTask, businessUnitsTask, usersTask);

        return new DashboardDto(
            BusinessCount: businessesTask.Result,
            BusinessUnitCount: businessUnitsTask.Result,
            UserCount: usersTask.Result,
            GeneratedAt: DateTime.UtcNow);
    }
}
