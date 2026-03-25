using Application.Abstractions.Messaging;
using Application.Authorization.Responses;

namespace Application.Authorization.Roles.GetAll;

public record GetRolesQuery : IQuery<List<RoleResponse>>;
