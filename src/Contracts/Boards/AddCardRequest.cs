using Contracts.Common;

namespace Contracts.Boards;

public record AddCardRequest(WorkItemType Type, string Title);