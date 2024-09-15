using System.Text.Json.Serialization;

namespace Mirai.Contracts.Common;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WorkItemType
{
    UserStory,
    Bug,
    Defect,
    Epic,
    Feature,
}