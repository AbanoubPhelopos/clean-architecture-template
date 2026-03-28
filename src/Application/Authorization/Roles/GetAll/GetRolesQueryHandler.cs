using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Authorization.Responses;
using Domain.Authorization;
using SharedKernel;

namespace Application.Authorization.Roles.GetAll;

internal sealed class GetRolesQueryHandler(IRoleRepository roleRepository) 
    : IQueryHandler<GetRolesQuery, PagedResult<RoleResponse>>
{
    public async Task<Result<PagedResult<RoleResponse>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        int skip = (request.Page - 1) * request.PageSize;
        
        IReadOnlyList<Role> roles = await roleRepository.GetAllAsync(cancellationToken);
        int totalCount = roles.Count;

        var pagedRoles = roles
            .Skip(skip)
            .Take(request.PageSize)
            .Select(r => new RoleResponse(
                r.Id,
                r.Name,
                r.Description,
                r.RolePermissions.Select(rp => rp.Permission.Name)
            )).ToList();

        var result = new PagedResult<RoleResponse>(pagedRoles, totalCount, request.Page, request.PageSize);

        return Result<PagedResult<RoleResponse>>.Success(result);
    }
}
