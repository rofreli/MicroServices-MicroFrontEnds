using MediatR;

namespace OAuth.Application.Commands.ActivateUser;

public record ActivateUserCommand(string Id) : IRequest;
