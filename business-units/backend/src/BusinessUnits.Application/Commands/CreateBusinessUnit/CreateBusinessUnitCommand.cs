using BusinessUnits.Application.DTOs;
using MediatR;

namespace BusinessUnits.Application.Commands.CreateBusinessUnit;

public record CreateBusinessUnitCommand(
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    AddressInputDto? Address,
    IReadOnlyList<ContactInputDto>? Contacts
) : IRequest<BusinessUnitDto>;
