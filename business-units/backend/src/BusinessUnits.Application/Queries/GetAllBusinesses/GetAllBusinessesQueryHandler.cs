using AutoMapper;
using BusinessUnits.Application.Common;
using BusinessUnits.Application.DTOs;
using BusinessUnits.Domain.Interfaces;
using MediatR;

namespace BusinessUnits.Application.Queries.GetAllBusinesses;

public class GetAllBusinessesQueryHandler
    : IRequestHandler<GetAllBusinessesQuery, PaginatedResult<BusinessSummaryDto>>
{
    private readonly IBusinessRepository _repository;
    private readonly IMapper _mapper;

    public GetAllBusinessesQueryHandler(IBusinessRepository repository, IMapper mapper)
        => (_repository, _mapper) = (repository, mapper);

    public async Task<PaginatedResult<BusinessSummaryDto>> Handle(
        GetAllBusinessesQuery request, CancellationToken ct)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var items = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);

        return new PaginatedResult<BusinessSummaryDto>(
            items.Select(_mapper.Map<BusinessSummaryDto>).ToList(),
            total, page, pageSize);
    }
}
