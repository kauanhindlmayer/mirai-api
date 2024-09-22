using ErrorOr;
using MediatR;
using Mirai.Domain.Retrospectives;

namespace Mirai.Application.Retrospectives.Commands.CreateRetrospective;

public record CreateRetrospectiveCommand(
    string Title,
    string Description,
    Guid TeamId) : IRequest<ErrorOr<Retrospective>>;