using Domain.Sprints;

namespace Domain.UnitTests.Sprints;

public static class SprintFactory
{
    public const string Name = "Sprint 1";
    public static readonly DateTime StartDate = DateTime.Now;
    public static readonly DateTime EndDate = DateTime.Now.AddDays(14);

    public static Sprint CreateSprint(
        Guid? teamId = null,
        string? name = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        return new(
            teamId ?? Guid.NewGuid(),
            name ?? Name,
            startDate ?? StartDate,
            endDate ?? EndDate);
    }
}