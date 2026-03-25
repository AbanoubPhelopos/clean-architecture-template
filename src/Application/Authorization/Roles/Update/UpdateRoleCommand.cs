using Application.Abstractions.Messaging;

namespace Application.Authorization.Roles.Update;

public record UpdateRoleCommand(Guid RoleId, string Name, string Description, IEnumerable<string> Permissions)
    : ICommand<RoleResponse>;

public record RoleResponse(Guid Id, string Name, string Description, IEnumerable<string> Permissions);