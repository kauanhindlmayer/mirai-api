namespace Application.Common.Interfaces.Services;

public interface IBackgroundJobScheduler
{
    Task ScheduleTagImportJobAsync(Guid importJobId, CancellationToken cancellationToken);
}