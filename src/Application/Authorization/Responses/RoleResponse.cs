namespace Application.Authorization.Responses;

public record RoleResponse(Guid Id, string Name, string Description, IEnumerable<string> Permissions);
