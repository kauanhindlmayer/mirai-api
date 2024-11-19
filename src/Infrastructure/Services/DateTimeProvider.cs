using Application.Common.Interfaces.Services;

namespace Infrastructure.Services;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
