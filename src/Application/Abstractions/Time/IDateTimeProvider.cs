namespace Application.Abstractions.Time;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}