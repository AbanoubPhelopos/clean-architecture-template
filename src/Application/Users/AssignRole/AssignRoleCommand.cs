using Application.Abstractions.Messaging;

namespace Application.Users.AssignRole;

public record AssignRoleCommand(Guid UserId, Guid RoleId) : ICommand<bool>;