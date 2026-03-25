using Application.Abstractions.Repositories;
using Domain.Authorization;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Database;

namespace Infrastructure.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
    }

    public async Task<IReadOnlyList<Role>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role)
            .ToListAsync(cancellationToken);
    }

    public async Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            AssignedAt = DateTime.UtcNow
        };
        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        UserRole? userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);

        if (userRole is not null)
        {
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> HasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    }
}