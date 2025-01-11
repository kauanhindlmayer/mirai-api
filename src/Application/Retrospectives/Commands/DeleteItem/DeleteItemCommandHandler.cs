using Application.Common.Interfaces.Persistence;
using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.DeleteItem;

internal sealed class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, ErrorOr<Success>>
{
    private readonly IRetrospectivesRepository _retrospectivesRepository;

    public DeleteItemCommandHandler(IRetrospectivesRepository retrospectivesRepository)
    {
        _retrospectivesRepository = retrospectivesRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteItemCommand command,
        CancellationToken cancellationToken)
    {
        var retrospective = await _retrospectivesRepository.GetByIdWithColumnsAsync(
            command.RetrospectiveId,
            cancellationToken);

        if (retrospective is null)
        {
            return RetrospectiveErrors.NotFound;
        }

        var column = retrospective.Columns.FirstOrDefault(c => c.Id == command.ColumnId);
        if (column is null)
        {
            return RetrospectiveErrors.ColumnNotFound;
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