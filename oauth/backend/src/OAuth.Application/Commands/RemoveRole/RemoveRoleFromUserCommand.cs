using MediatR;
using OAuth.Application.DTOs;

namespace OAuth.Application.Commands.RemoveRole;

public record RemovePermissionFromUserCommand(
    string UserId,
    string BusinessId,
    string? BusinessUnitId,
    string Module,
    string? Function
) : IRequest<UserDto>;
