using ErrorOr;
using MediatR;
using Mirai.Domain.Retrospectives;

namespace Mirai.Application.Retrospectives.Commands.CreateRetrospective;

public record CreateRetrospectiveCommand(
    string Title,
    string Description,
    Guid ProjectId) : IRequest<ErrorOr<Retrospective>>;