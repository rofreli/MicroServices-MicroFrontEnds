using AutoMapper;
using BusinessUnits.Application.DTOs;
using BusinessUnits.Domain.Entities;
using BusinessUnits.Domain.Exceptions;
using BusinessUnits.Domain.Interfaces;
using MediatR;

namespace BusinessUnits.Application.Commands.CreateBusinessUnit;

public class CreateBusinessUnitCommandHandler : IRequestHandler<CreateBusinessUnitCommand, BusinessUnitDto>
{
    private readonly IBusinessUnitRepository _repository;
    private readonly IMapper _mapper;

    public CreateBusinessUnitCommandHandler(IBusinessUnitRepository repository, IMapper mapper)
        => (_repository, _mapper) = (repository, mapper);

    public async Task<BusinessUnitDto> Handle(CreateBusinessUnitCommand request, CancellationToken ct)
    {
        if (await _repository.ExistsByCnpjAsync(request.Cnpj, ct))
            throw new DomainException($"A business unit with CNPJ '{request.Cnpj}' already exists.");

        var businessUnit = BusinessUnit.Create(request.RazaoSocial, request.NomeFantasia, request.Cnpj);

        if (request.Address is not null)
        {
            var address = Address.Create(
                request.Address.Street, request.Address.Number, request.Address.District,
                request.Address.City, request.Address.State, request.Address.ZipCode,
                request.Address.Country, request.Address.Complement);
            businessUnit.SetAddress(address);
        }

        if (request.Contacts is not null)
            foreach (var c in request.Contacts)
                businessUnit.AddContact(c.Name, c.Email, c.Phone, c.Type);

        await _repository.AddAsync(businessUnit, ct);
        return _mapper.Map<BusinessUnitDto>(businessUnit);
    }
}
