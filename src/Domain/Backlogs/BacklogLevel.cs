using System.Text.Json.Serialization;

namespace Domain.Backlogs;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BacklogLevel
{
    Epic,
    Feature,
    UserStory,
}