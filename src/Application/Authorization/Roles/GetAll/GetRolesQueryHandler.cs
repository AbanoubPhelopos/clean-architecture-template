using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Domain.Authorization;
using SharedKernel;

namespace Application.Authorization.Roles.GetAll;

internal sealed class GetRolesQueryHandler(IRoleRepository roleRepository) 
    : IQueryHandler<GetRolesQuery, List<RoleResponse>>
{
    public async Task<Result<List<RoleResponse>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Role> roles = await roleRepository.GetAllAsync(cancellationToken);

        var response = roles.Select(r => new RoleResponse(
            r.Id,
            r.Name,
            r.Description,
            r.RolePermissions.Select(rp => rp.Permission.Name)
        )).ToList();

        return Result<List<RoleResponse>>.Success(response);
    }
}
