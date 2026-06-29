using AutoMapper;
using MediatR;
using OAuth.Application.Common;
using OAuth.Application.DTOs;
using OAuth.Domain.Interfaces;

namespace OAuth.Application.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginatedResult<UserSummaryDto>>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(IUserRepository repository, IMapper mapper)
        => (_repository, _mapper) = (repository, mapper);

    public async Task<PaginatedResult<UserSummaryDto>> Handle(GetAllUsersQuery request, CancellationToken ct)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var items = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);
        return new PaginatedResult<UserSummaryDto>(
            items.Select(_mapper.Map<UserSummaryDto>).ToList(), total, page, pageSize);
    }
}
