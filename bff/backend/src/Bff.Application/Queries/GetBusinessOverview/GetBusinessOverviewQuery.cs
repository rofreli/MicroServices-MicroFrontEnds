using Bff.Application.Models;
using MediatR;

namespace Bff.Application.Queries.GetBusinessOverview;

public record GetBusinessOverviewQuery(
    string BusinessId,
    int BusinessUnitsPageSize = 20
) : IRequest<BusinessOverviewDto>;
