using Bff.Application.Abstractions;
using Bff.Application.Common;
using Bff.Application.Models;
using MediatR;

namespace Bff.Application.Queries.GetBusinessOverview;

/// <summary>
/// Composes a single business view from two domains: the business + its units come from
/// the Business Units domain, and the team (users with permissions on the business) from
/// the OAuth domain. Enforces that the caller may access the requested business.
/// </summary>
public class GetBusinessOverviewQueryHandler
    : IRequestHandler<GetBusinessOverviewQuery, BusinessOverviewDto>
{
    // Upper bound for the user fan-out when resolving the team of a business.
    private const int MaxTeamScan = 500;

    private readonly IBusinessCatalogApi _businessCatalog;
    private readonly IUserDirectoryApi _userDirectory;
    private readonly ICurrentUser _currentUser;

    public GetBusinessOverviewQueryHandler(
        IBusinessCatalogApi businessCatalog,
        IUserDirectoryApi userDirectory,
        ICurrentUser currentUser)
        => (_businessCatalog, _userDirectory, _currentUser) =
            (businessCatalog, userDirectory, currentUser);

    public async Task<BusinessOverviewDto> Handle(
        GetBusinessOverviewQuery request, CancellationToken ct)
    {
        // Authorization: never disclose a business the caller has no scope over.
        if (!_currentUser.CanAccessBusiness(request.BusinessId))
            throw new ForbiddenException(
                $"You do not have access to business '{request.BusinessId}'.");

        var pageSize = Math.Clamp(request.BusinessUnitsPageSize, 1, 100);

        var business = await _businessCatalog.GetBusinessAsync(request.BusinessId, ct)
            ?? throw new NotFoundException("Business", request.BusinessId);

        // Fan out the two remaining reads in parallel.
        var unitsTask = _businessCatalog.GetBusinessUnitsAsync(
            request.BusinessId, page: 1, pageSize: pageSize, ct);
        var teamTask = _userDirectory.GetUsersWithBusinessPermissionAsync(
            request.BusinessId, MaxTeamScan, ct);

        await Task.WhenAll(unitsTask, teamTask);

        var units = unitsTask.Result;
        var team = teamTask.Result;

        return new BusinessOverviewDto(
            Business: new BusinessSummaryDto(
                business.Id, business.RazaoSocial, business.NomeFantasia,
                business.Cnpj, business.IsActive, business.CreatedAt, business.UpdatedAt),
            BusinessUnitCount: units.TotalCount,
            BusinessUnits: units.Items
                .Select(u => new BusinessUnitItemDto(
                    u.Id, u.RazaoSocial, u.NomeFantasia, u.Cnpj, u.CreatedAt))
                .ToList(),
            Team: team
                .Select(u => new TeamMemberDto(
                    u.Id, u.Email, u.FullName, u.IsSuperAdmin,
                    u.Permissions
                        .Where(p => p.BusinessId == request.BusinessId)
                        .Select(p => p.Role)
                        .Distinct()
                        .OrderBy(r => r)
                        .ToList()))
                .OrderBy(m => m.FullName)
                .ToList());
    }
}
