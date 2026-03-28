using Application.Abstractions.Authentication;
using Domain.Authorization;
using Domain.Users;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data;

public sealed class DevelopmentSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<DevelopmentSeeder> _logger;

    public DevelopmentSeeder(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ILogger<DevelopmentSeeder> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _context.Users.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Database already seeded, skipping");
            return;
        }

        _logger.LogInformation("Seeding database...");

        var adminPermission = new Permission
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Name = "admin:full",
            Description = "Full administrative access"
        };

        var usersReadPermission = new Permission
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
            Name = "users:read",
            Description = "Read users"
        };

        var usersWritePermission = new Permission
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
            Name = "users:write",
            Description = "Create and update users"
        };

        var rolesReadPermission = new Permission
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
            Name = "roles:read",
            Description = "Read roles"
        };

        var rolesWritePermission = new Permission
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000005"),
            Name = "roles:write",
            Description = "Create, update and delete roles"
        };

        _context.Permissions.AddRange(
            adminPermission,
            usersReadPermission,
            usersWritePermission,
            rolesReadPermission,
            rolesWritePermission);

        var adminRole = new Role
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000010"),
            Name = "Admin",
            Description = "Administrator role with full access"
        };

        var userRole = new Role
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000011"),
            Name = "User",
            Description = "Standard user role"
        };

        _context.Roles.AddRange(adminRole, userRole);

        _context.RolePermissions.AddRange(
            new RolePermission { RoleId = adminRole.Id, PermissionId = adminPermission.Id },
            new RolePermission { RoleId = adminRole.Id, PermissionId = usersReadPermission.Id },
            new RolePermission { RoleId = adminRole.Id, PermissionId = usersWritePermission.Id },
            new RolePermission { RoleId = adminRole.Id, PermissionId = rolesReadPermission.Id },
            new RolePermission { RoleId = adminRole.Id, PermissionId = rolesWritePermission.Id },
            new RolePermission { RoleId = userRole.Id, PermissionId = usersReadPermission.Id }
        );

        var adminUser = new User
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000100"),
            Email = "admin@example.com",
            FirstName = "Admin",
            LastName = "User",
            PasswordHash = _passwordHasher.Hash("Admin123!")
        };

        _context.Users.Add(adminUser);

        _context.UserRoles.Add(new UserRole
        {
            UserId = adminUser.Id,
            RoleId = adminRole.Id
        });

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Database seeded successfully");
    }
}