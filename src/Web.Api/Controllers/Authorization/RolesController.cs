using Application.Abstractions.Messaging;
using Application.Authorization.Roles.Create;
using Application.Authorization.Roles.Delete;
using Application.Authorization.Roles.GetAll;
using Application.Authorization.Roles.GetById;
using Application.Authorization.Roles.Update;
using Application.Authorization.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Infrastructure;
using Web.Api.Extensions;

namespace Web.Api.Controllers.Authorization;

[ApiController]
[Route("roles")]
public class RolesController : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "roles:create")]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IResult> Create(
        [FromBody] CreateRoleRequest request,
        ICommandHandler<CreateRoleCommand, RoleResponse> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateRoleCommand(request.Name, request.Description, request.Permissions);
        Result<RoleResponse> result = await handler.Handle(command, cancellationToken);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet]
    [Authorize(Policy = "roles:read")]
    [ProducesResponseType(typeof(List<RoleResponse>), StatusCodes.Status200OK)]
    public async Task<IResult> GetAll(
        IQueryHandler<GetRolesQuery, List<RoleResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetRolesQuery();
        Result<List<RoleResponse>> result = await handler.Handle(query, cancellationToken);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("{roleId:guid}")]
    [Authorize(Policy = "roles:read")]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetById(
        [FromRoute] Guid roleId,
        IQueryHandler<GetRoleByIdQuery, RoleResponse> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetRoleByIdQuery(roleId);
        Result<RoleResponse> result = await handler.Handle(query, cancellationToken);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPut("{roleId:guid}")]
    [Authorize(Policy = "roles:update")]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> Update(
        [FromRoute] Guid roleId,
        [FromBody] UpdateRoleRequest request,
        ICommandHandler<UpdateRoleCommand, RoleResponse> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRoleCommand(roleId, request.Name, request.Description, request.Permissions);
        Result<RoleResponse> result = await handler.Handle(command, cancellationToken);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpDelete("{roleId:guid}")]
    [Authorize(Policy = "roles:delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> Delete(
        [FromRoute] Guid roleId,
        ICommandHandler<DeleteRoleCommand, bool> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteRoleCommand(roleId);
        Result<bool> result = await handler.Handle(command, cancellationToken);
        return result.Match(_ => Results.NoContent(), CustomResults.Problem);
    }
}

public sealed record CreateRoleRequest(string Name, string Description, IEnumerable<string> Permissions);
public sealed record UpdateRoleRequest(string Name, string Description, IEnumerable<string> Permissions);
