using AutoMapper;
using MediatR;
using OAuth.Application.DTOs;
using OAuth.Domain.Entities;
using OAuth.Domain.Exceptions;
using OAuth.Domain.Interfaces;

namespace OAuth.Application.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _hasher;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(IUserRepository repository, IPasswordHasher hasher, IMapper mapper)
        => (_repository, _hasher, _mapper) = (repository, hasher, mapper);

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken ct)
    {
        if (await _repository.ExistsByEmailAsync(request.Email, ct))
            throw new ConflictException($"User with email '{request.Email}' already exists.");

        var user = User.Create(request.Email, request.FirstName, request.LastName, _hasher.Hash(request.Password));

        foreach (var role in request.Roles ?? [])
            user.AddRole(role);

        await _repository.AddAsync(user, ct);
        return _mapper.Map<UserDto>(user);
    }
}
