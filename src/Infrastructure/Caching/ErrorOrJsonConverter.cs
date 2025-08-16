using System.Text.Json;
using System.Text.Json.Serialization;
using ErrorOr;

namespace Infrastructure.Caching;

public class ErrorOrJsonConverter<TValue> : JsonConverter<ErrorOr<TValue>>
{
    public override ErrorOr<TValue> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var dto = JsonSerializer.Deserialize<ErrorOrDto<TValue>>(ref reader, options);
        return dto is null ? default : (ErrorOr<TValue>)dto.Value!;
    }

    public override void Write(
        Utf8JsonWriter writer,
        ErrorOr<TValue> value,
        JsonSerializerOptions options)
    {
        var dto = new ErrorOrDto<TValue>
        {
            IsError = value.IsError,
            Errors = value.ErrorsOrEmptyList,
            Value = value.IsError ? default : value.Value,
            FirstError = value.FirstError,
        };

        JsonSerializer.Serialize(writer, dto, options);
    }

    private class ErrorOrDto<T>
    {
        public bool IsError { get; set; }
        public List<Error> Errors { get; set; } = [];
        public IReadOnlyList<Error> ErrorsOrEmptyList { get; set; } = [];
        public T? Value { get; set; }
        public Error? FirstError { get; set; }
    }
}
