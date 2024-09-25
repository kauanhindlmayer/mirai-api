using ErrorOr;
using MediatR;

namespace Mirai.Application.Tags.Commands.DeleteTag;

public record DeleteTagCommand(Guid ProjectId, string TagName)
    : IRequest<ErrorOr<Success>>;