using Application.Abstractions.Messaging;
using Application.Authorization.Responses;

namespace Application.Authorization.Roles.Update;

public record UpdateRoleCommand(Guid RoleId, string Name, string Description, IEnumerable<string> Permissions)
    : ICommand<RoleResponse>;
