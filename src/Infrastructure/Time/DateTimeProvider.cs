using Application.Abstractions.Time;

namespace Infrastructure.Time;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
