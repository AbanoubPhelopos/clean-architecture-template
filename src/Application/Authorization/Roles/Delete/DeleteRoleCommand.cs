using Application.Abstractions.Messaging;

namespace Application.Authorization.Roles.Delete;

public record DeleteRoleCommand(Guid RoleId) : ICommand<bool>;