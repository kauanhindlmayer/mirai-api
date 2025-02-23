using Domain.Sprints;

namespace Domain.UnitTests.Sprints;

public static class SprintFactory
{
    public static Sprint CreateSprint(
        Guid? teamId = null,
        string? name = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        return new(
            teamId ?? Guid.NewGuid(),
            name ?? $"Sprint 1",
            startDate ?? DateTime.Now,
            endDate ?? DateTime.Now.AddDays(14));
    }
}