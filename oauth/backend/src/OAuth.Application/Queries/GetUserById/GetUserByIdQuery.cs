using MediatR;
using OAuth.Application.DTOs;

namespace OAuth.Application.Queries.GetUserById;

public record GetUserByIdQuery(string Id) : IRequest<UserDto>;
