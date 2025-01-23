using System.Text.Json.Serialization;

namespace Domain.WorkItems.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WorkItemStatus
{
    New = 1,
    InProgress = 2,
    Closed = 3,
    Resolved = 4,
    Reopened = 5,
    Removed = 6,
}
