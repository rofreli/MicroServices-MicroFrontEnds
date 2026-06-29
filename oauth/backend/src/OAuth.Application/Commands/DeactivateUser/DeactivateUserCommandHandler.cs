using MediatR;
using OAuth.Domain.Exceptions;
using OAuth.Domain.Interfaces;

namespace OAuth.Application.Commands.DeactivateUser;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand>
{
    private readonly IUserRepository _repository;
    public DeactivateUserCommandHandler(IUserRepository repository) => _repository = repository;

    public async Task Handle(DeactivateUserCommand request, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("User", request.Id);
        user.Deactivate();
        await _repository.UpdateAsync(user, ct);
    }
}
