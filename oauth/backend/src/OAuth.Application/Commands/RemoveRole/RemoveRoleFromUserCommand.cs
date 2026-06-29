using MediatR;
using OAuth.Application.DTOs;

namespace OAuth.Application.Commands.RemoveRole;

public record RemoveRoleFromUserCommand(string UserId, string Role) : IRequest<UserDto>;
