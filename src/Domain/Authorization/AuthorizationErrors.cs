using SharedKernel;

namespace Domain.Authorization;

public static class AuthorizationErrors
{
    public static Error RoleNotFound(Guid roleId) =>
        new Error("Authorization.RoleNotFound", $"Role with ID '{roleId}' was not found.", ErrorType.NotFound);

    public static Error PermissionNotFound(Guid permissionId) =>
        new Error("Authorization.PermissionNotFound", $"Permission with ID '{permissionId}' was not found.", ErrorType.NotFound);

    public static Error RoleAlreadyExists(string roleName) =>
        new Error("Authorization.RoleAlreadyExists", $"Role with name '{roleName}' already exists.", ErrorType.Conflict);

    public static Error PermissionAlreadyExists(string permissionName) =>
        new Error("Authorization.PermissionAlreadyExists", $"Permission with name '{permissionName}' already exists.", ErrorType.Conflict);

    public static Error UserRoleNotFound(Guid userId, Guid roleId) =>
        new Error("Authorization.UserRoleNotFound", $"User '{userId}' does not have role '{roleId}'.", ErrorType.NotFound);

    public static Error CannotDeleteSystemRole =>
        new Error("Authorization.CannotDeleteSystemRole", "System roles cannot be deleted.", ErrorType.Problem);

    public static Error CannotAssignRoleToYourself =>
        new Error("Authorization.CannotAssignRoleToYourself", "Users cannot assign roles to themselves.", ErrorType.Problem);
}