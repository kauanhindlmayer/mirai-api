using Domain.Sprints;

namespace Domain.UnitTests.Sprints;

internal static class SprintData
{
    public const string Name = "Sprint 1";
    public static readonly Guid TeamId = Guid.NewGuid();
    public static readonly DateOnly StartDate = DateOnly.FromDateTime(DateTime.UtcNow);
    public static readonly DateOnly EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(14));

    public static Sprint Create(
        Guid? teamId = null,
        string? name = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null)
    {
        return new Sprint(
            teamId ?? TeamId,
            name ?? Name,
            startDate ?? StartDate,
            endDate ?? EndDate);
    }
}