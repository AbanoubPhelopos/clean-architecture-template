using Application.Abstractions.Messaging;
using Application.Todos.Complete;
using Application.Todos.Copy;
using Application.Todos.Create;
using Application.Todos.Delete;
using Application.Todos.Get;
using Application.Todos.GetById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers;

[ApiController]
[Route("todos")]
[Authorize]
public class TodosController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IResult> Create(
        [FromBody] CreateTodoRequest request,
        ICommandHandler<CreateTodoCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        if (!request.UserId.HasValue || !request.Priority.HasValue)
        {
            return Results.BadRequest();
        }

        var command = new CreateTodoCommand
        {
            UserId = request.UserId.Value,
            Description = request.Description,
            DueDate = request.DueDate,
            Labels = request.Labels,
            Priority = (Domain.Todos.Priority)request.Priority.Value
        };

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<Application.Todos.Get.TodoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetAll(
        [FromQuery] Guid userId,
        IQueryHandler<GetTodosQuery, List<Application.Todos.Get.TodoResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetTodosQuery(userId);

        Result<List<Application.Todos.Get.TodoResponse>> result = await handler.Handle(query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Application.Todos.GetById.TodoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetById(
        [FromRoute] Guid id,
        IQueryHandler<GetTodoByIdQuery, Application.Todos.GetById.TodoResponse> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetTodoByIdQuery(id);

        Result<Application.Todos.GetById.TodoResponse> result = await handler.Handle(query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPut("{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> Complete(
        [FromRoute] Guid id,
        ICommandHandler<CompleteTodoCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new CompleteTodoCommand(id);

        Result result = await handler.Handle(command, cancellationToken);

        return result.Match(() => Results.NoContent(), CustomResults.Problem);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> Delete(
        [FromRoute] Guid id,
        ICommandHandler<DeleteTodoCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteTodoCommand(id);

        Result result = await handler.Handle(command, cancellationToken);

        return result.Match(() => Results.NoContent(), CustomResults.Problem);
    }

    [HttpPost("{todoId:guid}/copy")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> Copy(
        [FromRoute] Guid todoId,
        [FromBody] CopyTodoRequest request,
        ICommandHandler<CopyTodoCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        if (!request.UserId.HasValue)
        {
            return Results.BadRequest();
        }

        var command = new CopyTodoCommand
        {
            UserId = request.UserId.Value,
            TodoId = todoId
        };

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
}

public sealed class CreateTodoRequest
{
    public Guid? UserId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public List<string> Labels { get; set; } = [];
    public int? Priority { get; set; }
}

public sealed class CopyTodoRequest
{
    public Guid? UserId { get; set; }
}