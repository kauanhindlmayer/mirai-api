using Application.Common.Interfaces.Persistence;
using Domain.TagImportJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.Jobs;

public sealed class CleanupTagImportJobsJob : IJob
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CleanupTagImportJobsJob> _logger;

    public CleanupTagImportJobsJob(
        IApplicationDbContext context,
        ILogger<CleanupTagImportJobsJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await DeleteCompletedJobsOlderThanDays(7);
            await DeleteFailedJobsOlderThanDays(30);
            await DeleteStuckJobsOlderThanHours(2);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while cleaning up tag import jobs");
        }
    }

    private async Task DeleteCompletedJobsOlderThanDays(int days)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        int count = await _context.TagImportJobs
            .Where(j => j.Status == TagImportJobStatus.Completed && j.CompletedAtUtc < cutoffDate)
            .ExecuteDeleteAsync();

        if (count > 0)
        {
            _logger.LogInformation("Deleted {Count} completed import jobs older than {Days} days", count, days);
        }
    }

    private async Task DeleteFailedJobsOlderThanDays(int days)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        int count = await _context.TagImportJobs
            .Where(j => j.Status == TagImportJobStatus.Failed && j.CompletedAtUtc < cutoffDate)
            .ExecuteDeleteAsync();

        if (count > 0)
        {
            _logger.LogInformation("Deleted {Count} failed import jobs older than {Days} days", count, days);
        }
    }

    private async Task DeleteStuckJobsOlderThanHours(int hours)
    {
        var cutoffDate = DateTime.UtcNow.AddHours(-hours);
        int count = await _context.TagImportJobs
            .Where(j => j.Status == TagImportJobStatus.Processing && j.CreatedAtUtc < cutoffDate)
            .ExecuteDeleteAsync();

        if (count > 0)
        {
            _logger.LogWarning("Deleted {Count} stuck import jobs older than {Hours} hours", count, hours);
        }
    }
}
