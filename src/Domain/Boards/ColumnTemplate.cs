namespace Domain.Boards;

public sealed record ColumnTemplate(
    string Name,
    int? WipLimit = null);
