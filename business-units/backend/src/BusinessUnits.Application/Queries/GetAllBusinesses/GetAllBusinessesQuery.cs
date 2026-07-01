using BusinessUnits.Application.Common;
using BusinessUnits.Application.DTOs;
using MediatR;

namespace BusinessUnits.Application.Queries.GetAllBusinesses;

public record GetAllBusinessesQuery(int Page = 1, int PageSize = 20)
    : IRequest<PaginatedResult<BusinessSummaryDto>>;
