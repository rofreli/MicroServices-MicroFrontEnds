using AutoMapper;
using MediatR;
using OAuth.Application.DTOs;
using OAuth.Domain.Exceptions;
using OAuth.Domain.Interfaces;

namespace OAuth.Application.Commands.RemoveRole;

public class RemovePermissionFromUserCommandHandler : IRequestHandler<RemovePermissionFromUserCommand, UserDto>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public RemovePermissionFromUserCommandHandler(IUserRepository repository, IMapper mapper)
        => (_repository, _mapper) = (repository, mapper);

    public async Task<UserDto> Handle(RemovePermissionFromUserCommand request, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException("User", request.UserId);

        user.RemovePermission(
            request.BusinessId, request.BusinessUnitId,
            request.Module, request.Function);

        await _repository.UpdateAsync(user, ct);
        return _mapper.Map<UserDto>(user);
    }
}
