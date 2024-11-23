namespace Contracts.Retrospectives;

public sealed record RetrospectiveResponse(
    Guid Id,
    string Name,
    string Description,
    List<RetrospectiveColumnResponse> Columns);

public sealed record RetrospectiveColumnResponse(
    Guid Id,
    string Title,
    int Position,
    List<RetrospectiveItemResponse> Items);

public sealed record RetrospectiveItemResponse(
    Guid Id,
    string Description,
    int Position,
    Guid AuthorId,
    int Votes);