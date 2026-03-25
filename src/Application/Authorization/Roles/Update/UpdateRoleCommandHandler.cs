using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Domain.Authorization;
using SharedKernel;

namespace Application.Authorization.Roles.Update;

internal sealed class UpdateRoleCommandHandler(
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository) : ICommandHandler<UpdateRoleCommand, RoleResponse>
{
    public async Task<Result<RoleResponse>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        Role? role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        
        if (role is null)
        {
            return Result.Failure<RoleResponse>(AuthorizationErrors.RoleNotFound(request.RoleId));
        }

        role.Name = request.Name;
        role.Description = request.Description;
        
        role.RolePermissions.Clear();

        IReadOnlyList<Permission> permissions = await permissionRepository.GetByNamesAsync(request.Permissions, cancellationToken);
        foreach (Permission permission in permissions)
        {
            role.RolePermissions.Add(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id
            });
        }

        roleRepository.Update(role);

        return new RoleResponse(
            role.Id,
            role.Name,
            role.Description,
            permissions.Select(p => p.Name));
    }
}
