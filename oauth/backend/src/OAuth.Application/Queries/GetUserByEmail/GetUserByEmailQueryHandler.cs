using AutoMapper;
using MediatR;
using OAuth.Application.DTOs;
using OAuth.Domain.Exceptions;
using OAuth.Domain.Interfaces;

namespace OAuth.Application.Queries.GetUserByEmail;

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserDto>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public GetUserByEmailQueryHandler(IUserRepository repository, IMapper mapper)
        => (_repository, _mapper) = (repository, mapper);

    public async Task<UserDto> Handle(GetUserByEmailQuery request, CancellationToken ct)
    {
        var user = await _repository.GetByEmailAsync(request.Email, ct)
            ?? throw new NotFoundException("User", request.Email);
        return _mapper.Map<UserDto>(user);
    }
}
