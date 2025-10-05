using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.DeleteRetrospective;

public sealed record DeleteRetrospectiveCommand(Guid RetrospectiveId) : IRequest<ErrorOr<Success>>;