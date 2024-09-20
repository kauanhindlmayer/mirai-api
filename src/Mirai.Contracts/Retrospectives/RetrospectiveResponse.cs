namespace Mirai.Contracts.Retrospectives;

public record RetrospectiveResponse(
    Guid Id,
    string Name,
    string Description,
    List<RetrospectiveColumnResponse> Columns,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record RetrospectiveColumnResponse(
    Guid Id,
    string Title,
    List<RetrospectiveItemResponse> Items);

public record RetrospectiveItemResponse(
    Guid Id,
    string Description,
    string Author,
    DateTime CreatedAt,
    DateTime? UpdatedAt);