using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Domain.Authorization;
using Domain.Users;
using SharedKernel;

namespace Application.Users.AssignRole;

internal sealed class AssignRoleCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository) : ICommandHandler<AssignRoleCommand, bool>
{
    public async Task<Result<bool>> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<bool>(UserErrors.NotFound(request.UserId));
        }

        Role? role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null)
        {
            return Result.Failure<bool>(AuthorizationErrors.RoleNotFound(request.RoleId));
        }

        bool hasRole = await userRepository.HasRoleAsync(request.UserId, request.RoleId, cancellationToken);
        if (hasRole)
        {
            return Result.Failure<bool>(Error.Conflict(
                "Authorization.UserAlreadyHasRole",
                "User already has this role assigned"));
        }

        await userRepository.AssignRoleAsync(request.UserId, request.RoleId, cancellationToken);

        return Result<bool>.Success(true);
    }
}
