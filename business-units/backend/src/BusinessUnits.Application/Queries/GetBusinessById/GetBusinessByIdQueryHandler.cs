using AutoMapper;
using BusinessUnits.Application.DTOs;
using BusinessUnits.Domain.Exceptions;
using BusinessUnits.Domain.Interfaces;
using MediatR;

namespace BusinessUnits.Application.Queries.GetBusinessById;

public class GetBusinessByIdQueryHandler : IRequestHandler<GetBusinessByIdQuery, BusinessDto>
{
    private readonly IBusinessRepository _repository;
    private readonly IMapper _mapper;

    public GetBusinessByIdQueryHandler(IBusinessRepository repository, IMapper mapper)
        => (_repository, _mapper) = (repository, mapper);

    public async Task<BusinessDto> Handle(GetBusinessByIdQuery request, CancellationToken ct)
    {
        var business = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Business", request.Id);
        return _mapper.Map<BusinessDto>(business);
    }
}
