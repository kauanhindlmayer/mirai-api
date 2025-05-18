using ErrorOr;
using MediatR;

namespace Application.Tags.Queries.ExportTags;

public sealed record ExportTagsQuery(Guid ProjectId) : IRequest<ErrorOr<byte[]>>;
