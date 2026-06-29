using AutoMapper;
using BusinessUnits.Application.Common;
using BusinessUnits.Application.DTOs;
using BusinessUnits.Domain.Interfaces;
using MediatR;

namespace BusinessUnits.Application.Queries.GetAllBusinessUnits;

public class GetAllBusinessUnitsQueryHandler
    : IRequestHandler<GetAllBusinessUnitsQuery, PaginatedResult<BusinessUnitSummaryDto>>
{
    private readonly IBusinessUnitRepository _repository;
    private readonly IMapper _mapper;

    public GetAllBusinessUnitsQueryHandler(IBusinessUnitRepository repository, IMapper mapper)
        => (_repository, _mapper) = (repository, mapper);

    public async Task<PaginatedResult<BusinessUnitSummaryDto>> Handle(
        GetAllBusinessUnitsQuery request, CancellationToken ct)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var items = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);

        return new PaginatedResult<BusinessUnitSummaryDto>(
            items.Select(_mapper.Map<BusinessUnitSummaryDto>).ToList(),
            total, page, pageSize
        );
    }
}
