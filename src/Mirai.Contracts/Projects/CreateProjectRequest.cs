namespace Mirai.Contracts.Projects;

public record CreateProjectRequest(string Name, string? Description, Guid OrganizationId);