using Domain.Retrospectives;
using Domain.Retrospectives.Enums;

namespace Domain.UnitTests.Retrospectives;

internal static class RetrospectiveFactory
{
    public static readonly string Title = $"Retrospective {DateTime.Now:MMM dd, yyyy}";
    public static readonly Guid TeamId = Guid.NewGuid();
    public static readonly int MaxVotesPerUser = 5;
    public static readonly RetrospectiveTemplate Template = RetrospectiveTemplate.Classic;

    public static Retrospective Create(
        string? title = null,
        int? maxVotesPerUser = null,
        RetrospectiveTemplate? template = null,
        Guid? teamId = null)
    {
        return new Retrospective(
            title ?? Title,
            teamId ?? TeamId,
            maxVotesPerUser ?? MaxVotesPerUser,
            template ?? Template);
    }
}