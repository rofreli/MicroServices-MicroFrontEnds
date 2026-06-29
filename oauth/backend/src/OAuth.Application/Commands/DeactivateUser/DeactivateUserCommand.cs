using MediatR;

namespace OAuth.Application.Commands.DeactivateUser;

public record DeactivateUserCommand(string Id) : IRequest;
