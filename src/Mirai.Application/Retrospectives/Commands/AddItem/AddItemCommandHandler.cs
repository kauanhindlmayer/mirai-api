using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Retrospectives;

namespace Mirai.Application.Retrospectives.Commands.AddItem;

public class AddItemCommandHandler(
    IRetrospectivesRepository _retrospectivesRepository,
    ICurrentUserProvider _currentUserProvider)
    : IRequestHandler<AddItemCommand, ErrorOr<Retrospective>>
{
    public async Task<ErrorOr<Retrospective>> Handle(
        AddItemCommand request,
        CancellationToken cancellationToken)
    {
        var retrospective = await _retrospectivesRepository.GetByIdAsync(
            request.RetrospectiveId,
            cancellationToken);

        if (retrospective is null)
        {
            return RetrospectiveErrors.RetrospectiveNotFound;
        }

        var currentUser = _currentUserProvider.GetCurrentUser();

        var retrospectiveItem = new RetrospectiveItem(
            retrospectiveColumnId: request.RetrospectiveColumnId,
            authorId: currentUser.Id,
            description: request.Description,
            votes: 0);

        var addedItemResult = retrospective.AddItem(retrospectiveItem, request.RetrospectiveColumnId);
        if (addedItemResult.IsError)
        {
            return addedItemResult.Errors;
        }

        await _retrospectivesRepository.UpdateAsync(retrospective, cancellationToken);

        return retrospective;
    }
}