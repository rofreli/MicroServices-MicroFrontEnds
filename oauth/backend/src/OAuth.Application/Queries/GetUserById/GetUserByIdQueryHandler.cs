using AutoMapper;
using MediatR;
using OAuth.Application.DTOs;
using OAuth.Domain.Exceptions;
using OAuth.Domain.Interfaces;

namespace OAuth.Application.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IUserRepository repository, IMapper mapper)
        => (_repository, _mapper) = (repository, mapper);

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("User", request.Id);
        return _mapper.Map<UserDto>(user);
    }
}
