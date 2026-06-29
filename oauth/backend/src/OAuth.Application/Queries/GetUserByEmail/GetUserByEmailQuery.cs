using MediatR;
using OAuth.Application.DTOs;

namespace OAuth.Application.Queries.GetUserByEmail;

public record GetUserByEmailQuery(string Email) : IRequest<UserDto>;
