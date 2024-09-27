using Mirai.Contracts.Common;

namespace Mirai.Contracts.Boards;

public record AddCardRequest(WorkItemType Type, string Title);