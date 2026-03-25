using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Authorization.Responses;
using Domain.Authorization;
using SharedKernel;

namespace Application.Authorization.Roles.GetById;

internal sealed class GetRoleByIdQueryHandler(IRoleRepository roleRepository) 
    : IQueryHandler<GetRoleByIdQuery, RoleResponse>
{
    public async Task<Result<RoleResponse>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        Role? role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        
        if (role is null)
        {
            return Result.Failure<RoleResponse>(AuthorizationErrors.RoleNotFound(request.RoleId));
        }

        var response = new RoleResponse(
            role.Id,
            role.Name,
            role.Description,
            role.RolePermissions.Select(rp => rp.Permission.Name));

        return Result<RoleResponse>.Success(response);
    }
}
