using BusinessUnits.Application.Commands.CreateBusiness;
using BusinessUnits.Application.Commands.DeleteBusiness;
using BusinessUnits.Application.Commands.UpdateBusiness;
using BusinessUnits.Application.Commands.CreateBusinessUnit;
using BusinessUnits.Application.DTOs;
using BusinessUnits.Application.Queries.GetAllBusinesses;
using BusinessUnits.Application.Queries.GetAllBusinessUnits;
using BusinessUnits.Application.Queries.GetBusinessById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BusinessUnits.API.Controllers;

[ApiController]
[Route("api/v1/businesses")]
[Produces("application/json")]
public class BusinessesController : ControllerBase
{
    private readonly IMediator _mediator;

    public BusinessesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetAllBusinessesQuery(page, pageSize), ct));

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetBusinessByIdQuery(id), ct));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateBusinessCommand command, CancellationToken ct = default)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        string id, [FromBody] UpdateBusinessBody body, CancellationToken ct = default)
        => Ok(await _mediator.Send(
            new UpdateBusinessCommand(id, body.RazaoSocial, body.NomeFantasia), ct));

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct = default)
    {
        await _mediator.Send(new DeleteBusinessCommand(id), ct);
        return NoContent();
    }

    // ── Nested Business Units ────────────────────────────────────────────────

    [HttpGet("{businessId}/business-units")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBusinessUnits(
        string businessId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetAllBusinessUnitsQuery(page, pageSize, businessId), ct));

    [HttpPost("{businessId}/business-units")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateBusinessUnit(
        string businessId,
        [FromBody] CreateBusinessUnitBody body,
        CancellationToken ct = default)
    {
        var command = new CreateBusinessUnitCommand(
            businessId, body.RazaoSocial, body.NomeFantasia, body.Cnpj,
            body.Address, body.Contacts);
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetBusinessUnits), new { businessId }, result);
    }
}

// The Id comes from the route; a dedicated body type keeps it out of model validation.
public record UpdateBusinessBody(string RazaoSocial, string NomeFantasia);

public record CreateBusinessUnitBody(
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    AddressInputDto? Address,
    IReadOnlyList<ContactInputDto>? Contacts
);
