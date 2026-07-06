using Bff.Application.Models;
using MediatR;

namespace Bff.Application.Queries.GetDashboard;

public record GetDashboardQuery : IRequest<DashboardDto>;
