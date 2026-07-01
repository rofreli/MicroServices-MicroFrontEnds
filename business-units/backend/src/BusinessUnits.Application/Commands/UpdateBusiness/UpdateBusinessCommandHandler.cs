using AutoMapper;
using BusinessUnits.Application.DTOs;
using BusinessUnits.Domain.Exceptions;
using BusinessUnits.Domain.Interfaces;
using MediatR;

namespace BusinessUnits.Application.Commands.UpdateBusiness;

public class UpdateBusinessCommandHandler : IRequestHandler<UpdateBusinessCommand, BusinessDto>
{
    private readonly IBusinessRepository _repository;
    private readonly IMapper _mapper;

    public UpdateBusinessCommandHandler(IBusinessRepository repository, IMapper mapper)
        => (_repository, _mapper) = (repository, mapper);

    public async Task<BusinessDto> Handle(UpdateBusinessCommand request, CancellationToken ct)
    {
        var business = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Business", request.Id);

        business.Update(request.RazaoSocial, request.NomeFantasia);
        await _repository.UpdateAsync(business, ct);
        return _mapper.Map<BusinessDto>(business);
    }
}
