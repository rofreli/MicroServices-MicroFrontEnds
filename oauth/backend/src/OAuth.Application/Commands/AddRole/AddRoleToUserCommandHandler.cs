using AutoMapper;
using MediatR;
using OAuth.Application.DTOs;
using OAuth.Domain.Entities;
using OAuth.Domain.Exceptions;
using OAuth.Domain.Interfaces;

namespace OAuth.Application.Commands.AddRole;

public class AddPermissionToUserCommandHandler : IRequestHandler<AddPermissionToUserCommand, UserDto>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public AddPermissionToUserCommandHandler(IUserRepository repository, IMapper mapper)
        => (_repository, _mapper) = (repository, mapper);

    public async Task<UserDto> Handle(AddPermissionToUserCommand request, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException("User", request.UserId);

        var permission = Permission.Create(
            request.BusinessId, request.BusinessUnitId,
            request.Module, request.Function, request.Role);

        user.AddPermission(permission);
        await _repository.UpdateAsync(user, ct);
        return _mapper.Map<UserDto>(user);
    }
}
