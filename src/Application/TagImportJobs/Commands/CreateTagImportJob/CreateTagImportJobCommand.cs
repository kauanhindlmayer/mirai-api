using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.TagImportJobs.Commands.CreateTagImportJob;

public sealed record CreateTagImportJobCommand(
    Guid ProjectId,
    IFormFile File) : IRequest<ErrorOr<Guid>>;