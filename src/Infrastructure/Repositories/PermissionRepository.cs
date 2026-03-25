using Application.Abstractions.Repositories;
using Domain.Authorization;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Database;

namespace Infrastructure.Repositories;

internal sealed class PermissionRepository : IPermissionRepository
{
    private readonly ApplicationDbContext _context;

    public PermissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .Where(p => names.Contains(p.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Permissions.ToListAsync(cancellationToken);
    }

    public async Task<Permission> AddAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync(cancellationToken);
        return permission;
    }

    public async Task<IReadOnlyList<Permission>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Include(rp => rp.Permission)
            .Select(rp => rp.Permission)
            .ToListAsync(cancellationToken);
    }
}