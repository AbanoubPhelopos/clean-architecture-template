using Application.Abstractions.Messaging;

namespace Application.Authorization.Roles.GetAll;

public record GetRolesQuery : IQuery<List<RoleResponse>>;

public record RoleResponse(Guid Id, string Name, string Description, IEnumerable<string> Permissions);
