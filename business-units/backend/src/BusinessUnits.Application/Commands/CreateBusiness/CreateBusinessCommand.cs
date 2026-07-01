using BusinessUnits.Application.DTOs;
using MediatR;

namespace BusinessUnits.Application.Commands.CreateBusiness;

public record CreateBusinessCommand(
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj
) : IRequest<BusinessDto>;
