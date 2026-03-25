using Application.Abstractions.Messaging;
using Domain.Authorization;

namespace Application.Authorization.Roles.Create;

public record CreateRoleCommand(string Name, string Description, IEnumerable<string> Permissions)
    : ICommand<RoleResponse>;

public record RoleResponse(Guid Id, string Name, string Description, IEnumerable<string> Permissions);