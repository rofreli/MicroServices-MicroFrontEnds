using BusinessUnits.Application.DTOs;
using MediatR;

namespace BusinessUnits.Application.Queries.GetBusinessUnitByCnpj;

public record GetBusinessUnitByCnpjQuery(string Cnpj) : IRequest<BusinessUnitDto>;
