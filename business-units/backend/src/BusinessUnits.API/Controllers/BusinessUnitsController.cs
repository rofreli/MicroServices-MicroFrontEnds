using BusinessUnits.Application.Commands.CreateBusinessUnit;
using BusinessUnits.Application.Commands.DeleteBusinessUnit;
using BusinessUnits.Application.Commands.UpdateBusinessUnit;
using BusinessUnits.Application.DTOs;
using BusinessUnits.Application.Queries.GetAllBusinessUnits;
using BusinessUnits.Application.Queries.GetBusinessUnitByCnpj;
using BusinessUnits.Application.Queries.GetBusinessUnitById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BusinessUnits.API.Controllers;

[ApiController]
[Route("api/v1/business-units")]
[Produces("application/json")]
public class BusinessUnitsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BusinessUnitsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetAllBusinessUnitsQuery(page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetBusinessUnitByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpGet("cnpj/{cnpj}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCnpj(string cnpj, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetBusinessUnitByCnpjQuery(cnpj), ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateBusinessUnitCommand command, CancellationToken ct = default)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateBusinessUnitCommand command, CancellationToken ct = default)
    {
        var result = await _mediator.Send(command with { Id = id }, ct);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct = default)
    {
        await _mediator.Send(new DeleteBusinessUnitCommand(id), ct);
        return NoContent();
    }
}
