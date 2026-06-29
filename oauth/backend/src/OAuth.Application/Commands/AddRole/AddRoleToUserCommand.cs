using MediatR;
using OAuth.Application.DTOs;

namespace OAuth.Application.Commands.AddRole;

public record AddRoleToUserCommand(string UserId, string Role) : IRequest<UserDto>;
