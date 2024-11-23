using Contracts.Common;

namespace Contracts.Boards;

public sealed record CreateCardRequest(
    WorkItemType Type,
    string Title);