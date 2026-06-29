using BusinessUnits.Domain.Exceptions;
using BusinessUnits.Domain.Interfaces;
using MediatR;

namespace BusinessUnits.Application.Commands.DeleteBusinessUnit;

public class DeleteBusinessUnitCommandHandler : IRequestHandler<DeleteBusinessUnitCommand>
{
    private readonly IBusinessUnitRepository _repository;

    public DeleteBusinessUnitCommandHandler(IBusinessUnitRepository repository)
        => _repository = repository;

    public async Task Handle(DeleteBusinessUnitCommand request, CancellationToken ct)
    {
        var exists = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("BusinessUnit", request.Id);

        await _repository.DeleteAsync(request.Id, ct);
    }
}
