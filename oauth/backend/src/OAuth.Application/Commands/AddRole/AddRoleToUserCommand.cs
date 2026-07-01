using MediatR;
using OAuth.Application.DTOs;

namespace OAuth.Application.Commands.AddRole;

public record AddPermissionToUserCommand(
    string UserId,
    string BusinessId,
    string? BusinessUnitId,
    string Module,
    string? Function,
    string Role
) : IRequest<UserDto>;
