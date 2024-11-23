using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateItem;

internal sealed class CreateItemCommandHandler(
    IRetrospectivesRepository retrospectivesRepository,
    IUserContext userContext)
    : IRequestHandler<CreateItemCommand, ErrorOr<RetrospectiveItem>>
{
    public async Task<ErrorOr<RetrospectiveItem>> Handle(
        CreateItemCommand command,
        CancellationToken cancellationToken)
    {
        var retrospective = await retrospectivesRepository.GetByIdWithColumnsAsync(
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
            userContext.UserId);

        var result = column.AddItem(retrospectiveItem);
        if (result.IsError)
        {
            return result.Errors;
        }

        retrospectivesRepository.Update(retrospective);

        return retrospectiveItem;
    }
}