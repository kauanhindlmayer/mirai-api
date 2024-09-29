using Application.Common.Interfaces;

namespace Infrastructure.Services;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
