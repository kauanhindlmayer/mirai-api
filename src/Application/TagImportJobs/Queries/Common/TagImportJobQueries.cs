using System.Linq.Expressions;
using Application.TagImportJobs.Queries.GetTagImportJob;
using Domain.TagImportJobs;

namespace Application.TagImportJobs.Queries.Common;

internal static class TagImportJobQueries
{
    public static Expression<Func<TagImportJob, TagImportJobResponse>> ProjectToDto()
    {
        return job => new TagImportJobResponse
        {
            Id = job.Id,
            Status = job.Status.ToString(),
            FileName = job.FileName,
            TotalRecords = job.TotalRecords,
            ProcessedRecords = job.ProcessedRecords,
            SuccessfulRecords = job.SuccessfulRecords,
            FailedRecords = job.FailedRecords,
            Errors = job.Errors,
            CreatedAtUtc = job.CreatedAtUtc,
            CompletedAtUtc = job.CompletedAtUtc,
        };
    }
}