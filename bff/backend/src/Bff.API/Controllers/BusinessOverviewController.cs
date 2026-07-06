using Bff.Application.Queries.GetBusinessOverview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bff.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/businesses")]
[Produces("application/json")]
public class BusinessOverviewController : ControllerBase
{
    private readonly IMediator _mediator;

    public BusinessOverviewController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Composite view of a business: its details, its business units and the team of
    /// users holding permissions on it. Requires access to the business.
    /// </summary>
    [HttpGet("{businessId}/overview")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> GetOverview(
        string businessId,
        [FromQuery] int businessUnitsPageSize = 20,
        CancellationToken ct = default)
        => Ok(await _mediator.Send(
            new GetBusinessOverviewQuery(businessId, businessUnitsPageSize), ct));
}
