using MediatR;
using Microsoft.AspNetCore.Mvc;
using OAuth.Application.Commands.ActivateUser;
using OAuth.Application.Commands.AddRole;
using OAuth.Application.Commands.CreateUser;
using OAuth.Application.Commands.DeactivateUser;
using OAuth.Application.Commands.RemoveRole;
using OAuth.Application.Commands.UpdateUser;
using OAuth.Application.Queries.GetAllUsers;
using OAuth.Application.Queries.GetUserByEmail;
using OAuth.Application.Queries.GetUserById;

namespace OAuth.API.Controllers;

[ApiController]
[Route("api/v1/users")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetAllUsersQuery(page, pageSize), ct));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetUserByIdQuery(id), ct));

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email, CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetUserByEmailQuery(email), ct));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserCommand command, CancellationToken ct = default)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id, [FromBody] UpdateUserRequest request, CancellationToken ct = default)
        => Ok(await _mediator.Send(
            new UpdateUserCommand(id, request.FirstName, request.LastName), ct));

    [HttpPatch("{id}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Deactivate(string id, CancellationToken ct = default)
    {
        await _mediator.Send(new DeactivateUserCommand(id), ct);
        return NoContent();
    }

    [HttpPatch("{id}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Activate(string id, CancellationToken ct = default)
    {
        await _mediator.Send(new ActivateUserCommand(id), ct);
        return NoContent();
    }

    // ── Permissions ──────────────────────────────────────────────────────────

    [HttpPost("{id}/permissions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddPermission(
        string id, [FromBody] AddPermissionRequest request, CancellationToken ct = default)
        => Ok(await _mediator.Send(new AddPermissionToUserCommand(
            id, request.BusinessId, request.BusinessUnitId,
            request.Module, request.Function, request.Role), ct));

    [HttpDelete("{id}/permissions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemovePermission(
        string id, [FromBody] RemovePermissionRequest request, CancellationToken ct = default)
        => Ok(await _mediator.Send(new RemovePermissionFromUserCommand(
            id, request.BusinessId, request.BusinessUnitId,
            request.Module, request.Function), ct));
}

// The Id comes from the route; a dedicated body type keeps it out of model validation.
public record UpdateUserRequest(string FirstName, string LastName);

public record AddPermissionRequest(
    string BusinessId,
    string? BusinessUnitId,
    string Module,
    string? Function,
    string Role
);

public record RemovePermissionRequest(
    string BusinessId,
    string? BusinessUnitId,
    string Module,
    string? Function
);
