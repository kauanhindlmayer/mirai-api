using Domain.Backlogs;

namespace Contracts.Backlogs;

public sealed record GetBacklogRequest(
    Guid? SprintId,
    BacklogLevel? BacklogLevel);