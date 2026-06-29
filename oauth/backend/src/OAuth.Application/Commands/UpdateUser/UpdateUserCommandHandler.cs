using AutoMapper;
using MediatR;
using OAuth.Application.DTOs;
using OAuth.Domain.Exceptions;
using OAuth.Domain.Interfaces;

namespace OAuth.Application.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(IUserRepository repository, IMapper mapper)
        => (_repository, _mapper) = (repository, mapper);

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("User", request.Id);
        user.Update(request.FirstName, request.LastName);
        await _repository.UpdateAsync(user, ct);
        return _mapper.Map<UserDto>(user);
    }
}
