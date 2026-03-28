using Application.Abstractions.Messaging;
using Application.Authorization.Responses;

namespace Application.Authorization.Roles.GetAll;

public record GetRolesQuery(int Page = 1, int PageSize = 10) : IPagedQuery<PagedResult<RoleResponse>>;
