using Application.Common.Interfaces;
using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateItem;

public class CreateItemCommandHandler(
    IRetrospectivesRepository _retrospectivesRepository,
    ICurrentUserProvider _currentUserProvider)
    : IRequestHandler<CreateItemCommand, ErrorOr<RetrospectiveItem>>
{
    public async Task<ErrorOr<RetrospectiveItem>> Handle(
        CreateItemCommand command,
        CancellationToken cancellationToken)
    {
        var retrospective = await _retrospectivesRepository.GetByIdWithColumnsAsync(
            command.RetrospectiveId,
            cancellationToken);

        if (retrospective is null)
        {
            return RetrospectiveErrors.RetrospectiveNotFound;
        }

        var column = retrospective.Columns.FirstOrDefault(c => c.Id == command.RetrospectiveColumnId);
        if (column is null)
        {
            return RetrospectiveErrors.RetrospectiveColumnNotFound;
        }

        var currentUser = _currentUserProvider.GetCurrentUser();

        var retrospectiveItem = new RetrospectiveItem(
            command.Description,
            command.RetrospectiveColumnId,
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