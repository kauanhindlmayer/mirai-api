using Contracts.Common;

namespace Contracts.Boards;

public record CreateCardRequest(WorkItemType Type, string Title);