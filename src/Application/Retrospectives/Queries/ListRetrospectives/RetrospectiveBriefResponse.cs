namespace Application.Retrospectives.Queries.ListRetrospectives;

public sealed class RetrospectiveBriefResponse
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
}