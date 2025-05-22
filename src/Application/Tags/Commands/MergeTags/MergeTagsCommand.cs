using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.MergeTags;

public sealed record MergeTagsCommand(
    Guid ProjectId,
    Guid TargetTagId,
    List<Guid> SourceTagIds) : IRequest<ErrorOr<Success>>;
