namespace Contracts.Retrospectives;

public record RetrospectiveResponse(
    Guid Id,
    string Name,
    string Description,
    List<RetrospectiveColumnResponse> Columns);

public record RetrospectiveColumnResponse(
    Guid Id,
    string Title,
    int Position,
    List<RetrospectiveItemResponse> Items);

public record RetrospectiveItemResponse(
    Guid Id,
    string Description,
    int Position,
    Guid AuthorId,
    int Votes);