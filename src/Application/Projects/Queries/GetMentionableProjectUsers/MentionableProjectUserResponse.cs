namespace Application.Projects.Queries.GetMentionableProjectUsers;

public sealed record MentionableProjectUserResponse(
    Guid Id,
    string FullName,
    string? ImageUrl);
