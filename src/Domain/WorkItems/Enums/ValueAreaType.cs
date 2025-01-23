using System.Text.Json.Serialization;

namespace Domain.WorkItems.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ValueAreaType
{
    Architectural = 1,
    Business = 2,
}
