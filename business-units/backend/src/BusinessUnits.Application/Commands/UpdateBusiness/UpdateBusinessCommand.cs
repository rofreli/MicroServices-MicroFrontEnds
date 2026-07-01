using BusinessUnits.Application.DTOs;
using MediatR;

namespace BusinessUnits.Application.Commands.UpdateBusiness;

public record UpdateBusinessCommand(
    string Id,
    string RazaoSocial,
    string NomeFantasia
) : IRequest<BusinessDto>;
