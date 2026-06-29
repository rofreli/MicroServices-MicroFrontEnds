using BusinessUnits.Application.DTOs;
using MediatR;

namespace BusinessUnits.Application.Queries.GetBusinessUnitById;

public record GetBusinessUnitByIdQuery(string Id) : IRequest<BusinessUnitDto>;
