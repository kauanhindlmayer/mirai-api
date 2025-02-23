using Domain.Retrospectives;
using Domain.Retrospectives.Enums;

namespace Domain.UnitTests.Retrospectives;

public static class RetrospectiveFactory
{
    public static Retrospective CreateRetrospective(
        string? title = null,
        int? maxVotesPerUser = null,
        RetrospectiveTemplate? template = null,
        Guid? teamId = null)
    {
        return new(
            title ?? $"Retrospective {DateTime.Now:MMM dd, yyyy}",
            maxVotesPerUser,
            template,
            teamId ?? Guid.NewGuid());
    }

    public static RetrospectiveColumn CreateColumn(
        string title = "Column",
        Guid? retrospectiveId = null)
    {
        return new(
            title,
            retrospectiveId ?? Guid.NewGuid());
    }
}