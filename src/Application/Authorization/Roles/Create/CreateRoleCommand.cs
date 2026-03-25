using Application.Abstractions.Messaging;
using Application.Authorization.Responses;

namespace Application.Authorization.Roles.Create;

public record CreateRoleCommand(string Name, string Description, IEnumerable<string> Permissions)
    : ICommand<RoleResponse>;
