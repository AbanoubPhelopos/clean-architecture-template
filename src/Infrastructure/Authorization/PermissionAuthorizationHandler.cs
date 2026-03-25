using Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Authorization;

internal sealed class PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User is not { Identity.IsAuthenticated: true })
        {
            return;
        }

        using IServiceScope scope = serviceScopeFactory.CreateScope();

        Application.Abstractions.Authorization.IAuthorizationService authorizationService = 
            scope.ServiceProvider.GetRequiredService<Application.Abstractions.Authorization.IAuthorizationService>();

        Guid userId = context.User.GetUserId();

        bool hasPermission = await authorizationService.HasPermissionAsync(userId, requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}
