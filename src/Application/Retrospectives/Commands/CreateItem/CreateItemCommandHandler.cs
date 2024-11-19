using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateItem;

public class CreateItemCommandHandler(
    IRetrospectivesRepository _retrospectivesRepository,
    IUserContext _userContext)
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
            return RetrospectiveErrors.NotFound;
        }

        var column = retrospective.Columns.FirstOrDefault(c => c.Id == command.RetrospectiveColumnId);
        if (column is null)
        {
            return RetrospectiveErrors.ColumnNotFound;
        }

        var retrospectiveItem = new RetrospectiveItem(
            command.Description,
            command.RetrospectiveColumnId,
            _userContext.UserId);

        var result = column.AddItem(retrospectiveItem);
        if (result.IsError)
        {
            return result.Errors;
        }

        _retrospectivesRepository.Update(retrospective);

        return retrospectiveItem;
    }
}