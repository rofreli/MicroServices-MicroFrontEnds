using AutoMapper;
using MediatR;
using OAuth.Application.DTOs;
using OAuth.Domain.Exceptions;
using OAuth.Domain.Interfaces;

namespace OAuth.Application.Commands.AddRole;

public class AddRoleToUserCommandHandler : IRequestHandler<AddRoleToUserCommand, UserDto>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public AddRoleToUserCommandHandler(IUserRepository repository, IMapper mapper)
        => (_repository, _mapper) = (repository, mapper);

    public async Task<UserDto> Handle(AddRoleToUserCommand request, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException("User", request.UserId);
        user.AddRole(request.Role);
        await _repository.UpdateAsync(user, ct);
        return _mapper.Map<UserDto>(user);
    }
}
