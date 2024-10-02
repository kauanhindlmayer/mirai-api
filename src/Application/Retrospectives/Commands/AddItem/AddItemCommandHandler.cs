using Application.Common.Interfaces;
using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.AddItem;

public class AddItemCommandHandler(
    IRetrospectivesRepository _retrospectivesRepository,
    ICurrentUserProvider _currentUserProvider)
    : IRequestHandler<AddItemCommand, ErrorOr<RetrospectiveItem>>
{
    public async Task<ErrorOr<RetrospectiveItem>> Handle(
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

        var column = retrospective.Columns.FirstOrDefault(c => c.Id == request.RetrospectiveColumnId);
        if (column is null)
        {
            return RetrospectiveErrors.RetrospectiveColumnNotFound;
        }

        var currentUser = _currentUserProvider.GetCurrentUser();

        var retrospectiveItem = new RetrospectiveItem(
            request.Description,
            request.RetrospectiveColumnId,
            currentUser.Id);

        var result = column.AddItem(retrospectiveItem);
        if (result.IsError)
        {
            return result.Errors;
        }

        _retrospectivesRepository.Update(retrospective);

        return retrospectiveItem;
    }
}