using Application.Abstractions.Authorization;
using Application.Abstractions.Repositories;
using Domain.Authorization;
using Domain.Users;

namespace Infrastructure.Authorization;

internal sealed class AuthorizationService : IAuthorizationService
{
    private readonly IUserRepository _userRepository;

    public AuthorizationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken = default)
    {
        User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return false;
        }

        return user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Any(rp => rp.Permission.Name == permission);
    }

    public async Task<HashSet<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return new HashSet<string>();
        }

        return user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .ToHashSet();
    }
}