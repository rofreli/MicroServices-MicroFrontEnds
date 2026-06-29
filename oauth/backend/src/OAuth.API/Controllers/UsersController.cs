using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        string id, [FromBody] UpdateUserCommand command, CancellationToken ct = default)
        => Ok(await _mediator.Send(command with { Id = id }, ct));

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

    [HttpPost("{id}/roles")]
    public async Task<IActionResult> AddRole(
        string id, [FromBody] RoleRequest request, CancellationToken ct = default)
        => Ok(await _mediator.Send(new AddRoleToUserCommand(id, request.Role), ct));

    [HttpDelete("{id}/roles/{role}")]
    public async Task<IActionResult> RemoveRole(
        string id, string role, CancellationToken ct = default)
        => Ok(await _mediator.Send(new RemoveRoleFromUserCommand(id, role), ct));
}

public record RoleRequest(string Role);
