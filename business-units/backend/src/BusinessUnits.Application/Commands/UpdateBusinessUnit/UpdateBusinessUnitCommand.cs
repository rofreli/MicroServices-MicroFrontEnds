using BusinessUnits.Application.DTOs;
using MediatR;

namespace BusinessUnits.Application.Commands.UpdateBusinessUnit;

public record UpdateBusinessUnitCommand(
    string Id,
    string RazaoSocial,
    string NomeFantasia,
    AddressInputDto? Address,
    IReadOnlyList<ContactInputDto>? Contacts
) : IRequest<BusinessUnitDto>;
