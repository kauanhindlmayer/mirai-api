namespace Contracts.Teams;

public sealed record TeamResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    List<MemberResponse> Members,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public sealed record MemberResponse(
    Guid Id,
    string Name);