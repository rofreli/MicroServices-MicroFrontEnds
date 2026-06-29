using AutoMapper;
using BusinessUnits.Application.DTOs;
using BusinessUnits.Domain.Exceptions;
using BusinessUnits.Domain.Interfaces;
using MediatR;

namespace BusinessUnits.Application.Queries.GetBusinessUnitByCnpj;

public class GetBusinessUnitByCnpjQueryHandler : IRequestHandler<GetBusinessUnitByCnpjQuery, BusinessUnitDto>
{
    private readonly IBusinessUnitRepository _repository;
    private readonly IMapper _mapper;

    public GetBusinessUnitByCnpjQueryHandler(IBusinessUnitRepository repository, IMapper mapper)
        => (_repository, _mapper) = (repository, mapper);

    public async Task<BusinessUnitDto> Handle(GetBusinessUnitByCnpjQuery request, CancellationToken ct)
    {
        var businessUnit = await _repository.GetByCnpjAsync(request.Cnpj, ct)
            ?? throw new NotFoundException("BusinessUnit", $"CNPJ {request.Cnpj}");

        return _mapper.Map<BusinessUnitDto>(businessUnit);
    }
}
