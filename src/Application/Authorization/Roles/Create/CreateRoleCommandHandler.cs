using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Domain.Authorization;
using SharedKernel;

namespace Application.Authorization.Roles.Create;

internal sealed class CreateRoleCommandHandler(
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository) : ICommandHandler<CreateRoleCommand, RoleResponse>
{
    public async Task<Result<RoleResponse>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (await roleRepository.ExistsAsync(request.Name, cancellationToken))
        {
            return Result.Failure<RoleResponse>(AuthorizationErrors.RoleAlreadyExists(request.Name));
        }

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        IReadOnlyList<Permission> permissions = await permissionRepository.GetByNamesAsync(request.Permissions, cancellationToken);
        foreach (Permission permission in permissions)
        {
            role.RolePermissions.Add(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id
            });
        }

        await roleRepository.AddAsync(role, cancellationToken);

        return new RoleResponse(
            role.Id,
            role.Name,
            role.Description,
            permissions.Select(p => p.Name));
    }
}
