using MediatR;
using OAuth.Domain.Exceptions;
using OAuth.Domain.Interfaces;

namespace OAuth.Application.Commands.ActivateUser;

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand>
{
    private readonly IUserRepository _repository;
    public ActivateUserCommandHandler(IUserRepository repository) => _repository = repository;

    public async Task Handle(ActivateUserCommand request, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("User", request.Id);
        user.Activate();
        await _repository.UpdateAsync(user, ct);
    }
}
