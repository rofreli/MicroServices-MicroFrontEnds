using AutoMapper;
using BusinessUnits.Application.DTOs;
using BusinessUnits.Domain.Entities;
using BusinessUnits.Domain.Exceptions;
using BusinessUnits.Domain.Interfaces;
using MediatR;

namespace BusinessUnits.Application.Commands.CreateBusiness;

public class CreateBusinessCommandHandler : IRequestHandler<CreateBusinessCommand, BusinessDto>
{
    private readonly IBusinessRepository _repository;
    private readonly IMapper _mapper;

    public CreateBusinessCommandHandler(IBusinessRepository repository, IMapper mapper)
        => (_repository, _mapper) = (repository, mapper);

    public async Task<BusinessDto> Handle(CreateBusinessCommand request, CancellationToken ct)
    {
        if (await _repository.ExistsByCnpjAsync(request.Cnpj, ct))
            throw new DomainException($"A business with CNPJ '{request.Cnpj}' already exists.");

        var business = Business.Create(request.RazaoSocial, request.NomeFantasia, request.Cnpj);
        await _repository.AddAsync(business, ct);
        return _mapper.Map<BusinessDto>(business);
    }
}
