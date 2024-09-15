using System.Text.Json.Serialization;

namespace Mirai.Contracts.Common;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WorkItemStatus
{
    New,
    InProgress,
    Closed,
    Resolved,
    Reopened,
}
