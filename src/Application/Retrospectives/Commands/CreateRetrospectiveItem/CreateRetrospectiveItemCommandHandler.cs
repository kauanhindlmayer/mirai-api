using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateRetrospectiveItem;

internal sealed class CreateRetrospectiveItemCommandHandler
    : IRequestHandler<CreateRetrospectiveItemCommand, ErrorOr<RetrospectiveItem>>
{
    private readonly IRetrospectivesRepository _retrospectivesRepository;
    private readonly IUserContext _userContext;

    public CreateRetrospectiveItemCommandHandler(
        IRetrospectivesRepository retrospectivesRepository,
        IUserContext userContext)
    {
        _retrospectivesRepository = retrospectivesRepository;
        _userContext = userContext;
    }

    public async Task<ErrorOr<RetrospectiveItem>> Handle(
        CreateRetrospectiveItemCommand command,
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

        var retrospectiveItem = new RetrospectiveItem(
            command.Description,
            command.ColumnId,
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