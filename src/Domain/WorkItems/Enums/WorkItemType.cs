using System.Text.Json.Serialization;

namespace Domain.WorkItems.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WorkItemType
{
    Epic = 1,
    Feature = 2,
    UserStory = 3,
    Bug = 4,
    Defect = 5,
}