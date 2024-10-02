using Application.Common.Interfaces;
using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.DeleteItem;

public class DeleteItemCommandHandler(IRetrospectivesRepository _retrospectivesRepository)
    : IRequestHandler<DeleteItemCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteItemCommand command,
        CancellationToken cancellationToken)
    {
        var retrospective = await _retrospectivesRepository.GetByIdWithColumnsAsync(
            command.RetrospectiveId,
            cancellationToken);

        if (retrospective is null)
        {
            return RetrospectiveErrors.RetrospectiveNotFound;
        }

        var column = retrospective.Columns.FirstOrDefault(c => c.Id == command.ColumnId);
        if (column is null)
        {
            return RetrospectiveErrors.RetrospectiveColumnNotFound;
        }

        var result = column.RemoveItem(command.ItemId);
        if (result.IsError)
        {
            return result.Errors;
        }

        _retrospectivesRepository.Update(retrospective);

        return Result.Success;
    }
}