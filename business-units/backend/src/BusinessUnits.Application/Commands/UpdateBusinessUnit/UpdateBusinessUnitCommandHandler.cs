using AutoMapper;
using BusinessUnits.Application.DTOs;
using BusinessUnits.Domain.Entities;
using BusinessUnits.Domain.Exceptions;
using BusinessUnits.Domain.Interfaces;
using MediatR;

namespace BusinessUnits.Application.Commands.UpdateBusinessUnit;

public class UpdateBusinessUnitCommandHandler : IRequestHandler<UpdateBusinessUnitCommand, BusinessUnitDto>
{
    private readonly IBusinessUnitRepository _repository;
    private readonly IMapper _mapper;

    public UpdateBusinessUnitCommandHandler(IBusinessUnitRepository repository, IMapper mapper)
        => (_repository, _mapper) = (repository, mapper);

    public async Task<BusinessUnitDto> Handle(UpdateBusinessUnitCommand request, CancellationToken ct)
    {
        var businessUnit = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("BusinessUnit", request.Id);

        businessUnit.Update(request.RazaoSocial, request.NomeFantasia);

        if (request.Address is not null)
        {
            var address = Address.Create(
                request.Address.Street, request.Address.Number, request.Address.District,
                request.Address.City, request.Address.State, request.Address.ZipCode,
                request.Address.Country, request.Address.Complement);
            businessUnit.SetAddress(address);
        }

        if (request.Contacts is not null)
        {
            foreach (var contact in businessUnit.Contacts.ToList())
                businessUnit.RemoveContact(contact.Id);
            foreach (var c in request.Contacts)
                businessUnit.AddContact(c.Name, c.Email, c.Phone, c.Type);
        }

        await _repository.UpdateAsync(businessUnit, ct);
        return _mapper.Map<BusinessUnitDto>(businessUnit);
    }
}
