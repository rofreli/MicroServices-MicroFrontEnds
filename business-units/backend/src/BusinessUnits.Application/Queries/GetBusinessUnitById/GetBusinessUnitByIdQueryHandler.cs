using AutoMapper;
using BusinessUnits.Application.DTOs;
using BusinessUnits.Domain.Exceptions;
using BusinessUnits.Domain.Interfaces;
using MediatR;

namespace BusinessUnits.Application.Queries.GetBusinessUnitById;

public class GetBusinessUnitByIdQueryHandler : IRequestHandler<GetBusinessUnitByIdQuery, BusinessUnitDto>
{
    private readonly IBusinessUnitRepository _repository;
    private readonly IMapper _mapper;

    public GetBusinessUnitByIdQueryHandler(IBusinessUnitRepository repository, IMapper mapper)
        => (_repository, _mapper) = (repository, mapper);

    public async Task<BusinessUnitDto> Handle(GetBusinessUnitByIdQuery request, CancellationToken ct)
    {
        var businessUnit = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("BusinessUnit", request.Id);

        return _mapper.Map<BusinessUnitDto>(businessUnit);
    }
}
