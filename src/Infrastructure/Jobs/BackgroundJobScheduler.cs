using Application.Abstractions.Jobs;
using Quartz;

namespace Infrastructure.Jobs;

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

    public async Task ScheduleGitHubPullRequestWebhookJobAsync(
        GitHubPullRequestWebhookPayload payload,
        CancellationToken cancellationToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        var jobId = Guid.NewGuid();

        var job = JobBuilder.Create<ProcessGitHubPullRequestWebhookJob>()
            .WithIdentity($"process-github-pr-webhook-{jobId}")
            .UsingJobData(new JobDataMap { { "payload", payload } })
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"trigger-github-pr-webhook-{jobId}")
            .StartNow()
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }
}