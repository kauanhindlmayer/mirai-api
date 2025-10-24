using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Presentation.Converters;

public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string DefaultFormat = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new JsonException("DateOnly value cannot be null or empty.");
        }

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dateTime))
        {
            return DateOnly.FromDateTime(dateTime);
        }

        throw new JsonException($"Unable to parse '{value}' as a valid date.");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DefaultFormat, CultureInfo.InvariantCulture));
    }
}
