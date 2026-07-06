using Bff.Application.Queries.GetDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bff.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/dashboard")]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator) => _mediator = mediator;

    /// <summary>Cross-domain landing summary (business, business-unit and user counts).</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> Get(CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetDashboardQuery(), ct));
}
