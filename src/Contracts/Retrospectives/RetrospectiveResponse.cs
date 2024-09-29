namespace Contracts.Retrospectives;

public record RetrospectiveResponse(
    Guid Id,
    string Name,
    string Description,
    List<RetrospectiveColumnResponse> Columns);

public record RetrospectiveColumnResponse(
    Guid Id,
    string Title,
    List<RetrospectiveItemResponse> Items);

public record RetrospectiveItemResponse(
    Guid Id,
    string Description,
    Guid AuthorId,
    int Votes);