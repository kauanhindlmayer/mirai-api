namespace Application.Retrospectives.Queries.ListRetrospectives;

public sealed class RetrospectiveBriefResponse
{
     public Guid Id { get; init; }
     public string Title { get; init; } = string.Empty;
}