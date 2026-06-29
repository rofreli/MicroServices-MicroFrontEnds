using MediatR;
using OAuth.Application.Common;
using OAuth.Application.DTOs;

namespace OAuth.Application.Queries.GetAllUsers;

public record GetAllUsersQuery(int Page = 1, int PageSize = 20) : IRequest<PaginatedResult<UserSummaryDto>>;
