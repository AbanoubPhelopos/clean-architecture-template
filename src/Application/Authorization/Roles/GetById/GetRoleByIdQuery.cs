using Application.Abstractions.Messaging;
using Application.Authorization.Responses;

namespace Application.Authorization.Roles.GetById;

public record GetRoleByIdQuery(Guid RoleId) : IQuery<RoleResponse>;
