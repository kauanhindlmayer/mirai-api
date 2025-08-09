using Application.Abstractions.Authentication;
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

        var retrospectiveItem = new RetrospectiveItem(
            command.Content,
            command.ColumnId,
            _userContext.UserId);

        var result = retrospective.AddItem(retrospectiveItem);
        if (result.IsError)
        {
            return result.Errors;
        }

        _retrospectivesRepository.Update(retrospective);

        return retrospectiveItem;
    }
}