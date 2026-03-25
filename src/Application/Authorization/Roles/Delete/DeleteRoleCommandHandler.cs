using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Domain.Authorization;
using SharedKernel;

namespace Application.Authorization.Roles.Delete;

internal sealed class DeleteRoleCommandHandler(IRoleRepository roleRepository) 
    : ICommandHandler<DeleteRoleCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        Role? role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        
        if (role is null)
        {
            return Result.Failure<bool>(AuthorizationErrors.RoleNotFound(request.RoleId));
        }

        roleRepository.Delete(role);

        return true;
    }
}
