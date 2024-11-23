namespace Contracts.Retrospectives;

public sealed record CreateRetrospectiveRequest(
    string Title,
    string Description);