using Application.Abstractions.Messaging;

namespace Application.Authorization.Roles.GetById;

public record GetRoleByIdQuery(Guid RoleId) : IQuery<RoleResponse>;

public record RoleResponse(Guid Id, string Name, string Description, IEnumerable<string> Permissions);
