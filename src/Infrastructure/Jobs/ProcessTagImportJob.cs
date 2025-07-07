using System.Globalization;
using Application.Common.Interfaces.Persistence;
using CsvHelper;
using Domain.Tags;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.Jobs;

public sealed class ProcessTagImportJob : IJob
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ProcessTagImportJob> _logger;

    public ProcessTagImportJob(
        IApplicationDbContext context,
        ILogger<ProcessTagImportJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var importJobId = context.MergedJobDataMap.GetString("importJobId")!;
        var importJob = _context.TagImportJobs.FirstOrDefault(j => j.Id.ToString() == importJobId);

        if (importJob is null)
        {
            _logger.LogError("Tag import job with ID {ImportJobId} not found.", importJobId);
            return;
        }

        try
        {
            importJob.StartProcessing();
            await _context.SaveChangesAsync(context.CancellationToken);

            using var memoryStream = new MemoryStream(importJob.FileContent);
            using var reader = new StreamReader(memoryStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<CsvTagRecord>().ToList();

            importJob.SetTotalRecords(records.Count);
            await _context.SaveChangesAsync(context.CancellationToken);

            foreach (var record in records)
            {
                try
                {
                    var existingTag = _context.Tags
                        .FirstOrDefault(t => t.Name == record.Name && t.ProjectId == record.ProjectId);

                    if (existingTag is not null)
                    {
                        importJob.IncrementSuccessfulRecords();
                        _logger.LogInformation("Tag '{TagName}' already exists, skipping.", record.Name);
                        continue;
                    }

                    var tag = new Tag(
                        record.Name,
                        record.Description,
                        record.Color,
                        record.ProjectId);

                    _context.Tags.Add(tag);
                    importJob.IncrementSuccessfulRecords();
                }
                catch (Exception ex)
                {
                    importJob.IncrementFailedRecords();
                    importJob.AddError($"Error processing record: {ex.Message}");

                    if (importJob.Errors.Count >= 100)
                    {
                        importJob.AddError("Too many errors encountered, stopping processing.");
                        break;
                    }
                }
                finally
                {
                    importJob.IncrementProcessedRecords();
                }

                if (importJob.ProcessedRecords % 100 == 0)
                {
                    await _context.SaveChangesAsync();
                }
            }

            importJob.CompleteProcessing();
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process tag import job with ID {ImportJobId}.", importJobId);
            importJob.FailProcessing(ex.Message);
            await _context.SaveChangesAsync();
        }
    }
}
