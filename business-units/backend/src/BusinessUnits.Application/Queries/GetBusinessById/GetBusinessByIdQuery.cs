using BusinessUnits.Application.DTOs;
using MediatR;

namespace BusinessUnits.Application.Queries.GetBusinessById;

public record GetBusinessByIdQuery(string Id) : IRequest<BusinessDto>;
