using MediatR;
using OAuth.Application.DTOs;

namespace OAuth.Application.Commands.CreateUser;

public record CreateUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password,
    IReadOnlyList<string>? Roles
) : IRequest<UserDto>;
