namespace Domain.Authorization;

public sealed class UserRole
{
    public Guid UserId { get; set; }
    public Domain.Users.User User { get; set; }

    public Guid RoleId { get; set; }
    public Role Role { get; set; }

    public DateTime AssignedAt { get; set; }
}