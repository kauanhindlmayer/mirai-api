namespace Mirai.Contracts.Teams;

public record TeamResponse(
    Guid Id,
    string Name,
    Guid ProjectId,
    List<MemberResponse> Members,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record MemberResponse(Guid Id, string Name);