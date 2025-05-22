using Domain.Sprints;

namespace Domain.UnitTests.Sprints;

public static class SprintFactory
{
    public const string Name = "Sprint 1";
    public static readonly DateOnly StartDate = DateOnly.FromDateTime(DateTime.UtcNow);
    public static readonly DateOnly EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(14));

    public static Sprint CreateSprint(
        Guid? teamId = null,
        string? name = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null)
    {
        return new(
            teamId ?? Guid.NewGuid(),
            name ?? Name,
            startDate ?? StartDate,
            endDate ?? EndDate);
    }
}