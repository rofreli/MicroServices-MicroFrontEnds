using MediatR;
using OAuth.Application.DTOs;

namespace OAuth.Application.Commands.UpdateUser;

public record UpdateUserCommand(string Id, string FirstName, string LastName) : IRequest<UserDto>;
