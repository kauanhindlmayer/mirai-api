using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.DeleteTag;

public record DeleteTagCommand(Guid ProjectId, string TagName)
    : IRequest<ErrorOr<Success>>;