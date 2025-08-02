using ErrorOr;
using MediatR;

namespace Application.WisdomExtractor.Queries.ExtractWisdom;

public sealed record ExtractWisdomQuery(
    Guid ProjectId,
    string Question) : IRequest<ErrorOr<WisdomResponse>>;