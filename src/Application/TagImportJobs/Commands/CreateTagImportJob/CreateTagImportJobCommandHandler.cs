using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.TagImportJobs;
using ErrorOr;
using MediatR;

namespace Application.TagImportJobs.Commands.CreateTagImportJob;

internal sealed class CreateTagImportJobCommandHandler
    : IRequestHandler<CreateTagImportJobCommand, ErrorOr<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBackgroundJobScheduler _backgroundJobScheduler;

    public CreateTagImportJobCommandHandler(
        IApplicationDbContext context,
        IBackgroundJobScheduler backgroundJobScheduler)
    {
        _context = context;
        _backgroundJobScheduler = backgroundJobScheduler;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateTagImportJobCommand command,
        CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        await command.File.CopyToAsync(memoryStream, cancellationToken);

        var importJob = new TagImportJob(
            command.ProjectId,
            command.File.FileName,
            memoryStream.ToArray());

        await _context.TagImportJobs.AddAsync(importJob, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _backgroundJobScheduler.ScheduleTagImportJobAsync(
            importJob.Id,
            cancellationToken);

        return importJob.Id;
    }
}