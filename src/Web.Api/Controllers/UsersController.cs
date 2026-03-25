using System.Text.Json.Serialization;
using Application.Abstractions.Messaging;
using Application.Users.AssignRole;
using Application.Users.GetById;
using Application.Users.Login;
using Application.Users.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IResult> Register(
        [FromBody] RegisterRequest request,
        ICommandHandler<RegisterUserCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.FirstName,
            request.LastName,
            request.Password);

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> Login(
        [FromBody] LoginRequest request,
        ICommandHandler<LoginUserCommand, string> handler,
        CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(request.Email, request.Password);

        Result<string> result = await handler.Handle(command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("{userId:guid}")]
    [Authorize(Policy = "users:read")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetById(
        [FromRoute] Guid userId,
        IQueryHandler<GetUserByIdQuery, UserResponse> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(userId);

        Result<UserResponse> result = await handler.Handle(query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost("{userId:guid}/roles")]
    [Authorize(Policy = "users:assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IResult> AssignRole(
        [FromRoute] Guid userId,
        [FromBody] AssignRoleRequest request,
        ICommandHandler<AssignRoleCommand, bool> handler,
        CancellationToken cancellationToken)
    {
        var command = new AssignRoleCommand(userId, request.RoleId);
        Result<bool> result = await handler.Handle(command, cancellationToken);
        return result.Match(_ => Results.Ok(), CustomResults.Problem);
    }
}

public sealed record RegisterRequest(string Email, string FirstName, string LastName, string Password);
public sealed record LoginRequest(string Email, string Password);

public sealed class AssignRoleRequest
{
    [JsonPropertyName("roleId")]
    public required Guid RoleId { get; init; }
}