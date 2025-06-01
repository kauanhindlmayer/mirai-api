using Application.Common.Interfaces.Services;
using Infrastructure.Jobs;
using Quartz;

namespace Infrastructure.Services;

internal sealed class BackgroundJobScheduler : IBackgroundJobScheduler
{
    private readonly ISchedulerFactory _schedulerFactory;

    public BackgroundJobScheduler(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task ScheduleTagImportJobAsync(Guid importJobId, CancellationToken cancellationToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        var job = JobBuilder.Create<ProcessTagImportJob>()
            .WithIdentity($"process-tag-import-{importJobId}")
            .UsingJobData("importJobId", importJobId.ToString())
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"trigger-tag-import-{importJobId}")
            .StartNow()
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }
}