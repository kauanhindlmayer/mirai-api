using Domain.Retrospectives.Enums;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.UpdateRetrospective;

public sealed record UpdateRetrospectiveCommand(
    Guid RetrospectiveId,
    string Title,
    int? MaxVotesPerUser,
    RetrospectiveTemplate? Template) : IRequest<ErrorOr<Guid>>;