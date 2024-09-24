using ErrorOr;
using MediatR;

namespace Mirai.Application.Projects.Commands.DeleteTag;

public record DeleteTagCommand(Guid ProjectId, string TagName)
    : IRequest<ErrorOr<Success>>;