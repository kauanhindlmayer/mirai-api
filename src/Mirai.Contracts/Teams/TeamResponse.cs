namespace Mirai.Contracts.Teams;

public record TeamResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    List<MemberResponse> Members,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record MemberResponse(Guid Id, string Name);