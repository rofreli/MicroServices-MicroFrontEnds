using BusinessUnits.Domain.Exceptions;
using BusinessUnits.Domain.Interfaces;
using MediatR;

namespace BusinessUnits.Application.Commands.DeleteBusiness;

public class DeleteBusinessCommandHandler : IRequestHandler<DeleteBusinessCommand>
{
    private readonly IBusinessRepository _repository;
    private readonly IBusinessUnitRepository _businessUnitRepository;

    public DeleteBusinessCommandHandler(
        IBusinessRepository repository,
        IBusinessUnitRepository businessUnitRepository)
        => (_repository, _businessUnitRepository) = (repository, businessUnitRepository);

    public async Task Handle(DeleteBusinessCommand request, CancellationToken ct)
    {
        _ = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Business", request.Id);

        var buCount = await _businessUnitRepository.CountAsync(request.Id, ct);
        if (buCount > 0)
            throw new DomainException(
                $"Cannot delete Business '{request.Id}': it still has {buCount} Business Unit(s). Remove them first.");

        await _repository.DeleteAsync(request.Id, ct);
    }
}
