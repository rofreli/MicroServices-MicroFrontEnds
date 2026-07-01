using BusinessUnits.Application.Common;
using BusinessUnits.Application.DTOs;
using MediatR;

namespace BusinessUnits.Application.Queries.GetAllBusinessUnits;

public record GetAllBusinessUnitsQuery(
    int Page = 1,
    int PageSize = 20,
    string? BusinessId = null
) : IRequest<PaginatedResult<BusinessUnitSummaryDto>>;
