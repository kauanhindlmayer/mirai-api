using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;

namespace Mirai.Application.Projects.Commands.DeleteTag;

public class DeleteTagCommandHandler(ITagsRepository _tagsRepository)
    : IRequestHandler<DeleteTagCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteTagCommand request,
        CancellationToken cancellationToken)
    {
        var tag = await _tagsRepository.GetByNameAsync(
            request.TagName,
            cancellationToken);

        if (tag is null)
        {
            return Error.NotFound(description: "Tag not found.");
        }

        // TODO: Refactor the tag to make it belong to a project and not be global.
        await _tagsRepository.RemoveAsync(tag, cancellationToken);

        return Result.Success;
    }
}