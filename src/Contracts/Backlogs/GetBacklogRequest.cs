using Domain.Backlogs;

namespace Contracts.Backlogs;

public sealed record GetBacklogRequest(BacklogLevel? BacklogLevel);