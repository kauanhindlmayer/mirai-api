using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.CreateTag;

public record CreateTagCommand(Guid ProjectId, string Name)
    : IRequest<ErrorOr<Tag>>;