using System.Text.Json.Serialization;

namespace Domain.Boards.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProcessTemplate
{
    Basic = 1,
    Agile = 2,
    Scrum = 3,
}