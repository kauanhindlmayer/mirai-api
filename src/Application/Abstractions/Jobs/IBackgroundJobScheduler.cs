namespace Application.Abstractions.Jobs;

public interface IBackgroundJobScheduler
{
    Task ScheduleTagImportJobAsync(Guid importJobId, CancellationToken cancellationToken);
}