using Application.Abstractions.Authentication;
using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateRetrospectiveItem;

internal sealed class CreateRetrospectiveItemCommandHandler
    : IRequestHandler<CreateRetrospectiveItemCommand, ErrorOr<RetrospectiveItem>>
{
    private readonly IRetrospectiveRepository _retrospectiveRepository;
    private readonly IUserContext _userContext;

    public CreateRetrospectiveItemCommandHandler(
        IRetrospectiveRepository retrospectiveRepository,
        IUserContext userContext)
    {
        _retrospectiveRepository = retrospectiveRepository;
        _userContext = userContext;
    }

    public async Task<ErrorOr<RetrospectiveItem>> Handle(
        CreateRetrospectiveItemCommand command,
        CancellationToken cancellationToken)
    {
        var retrospective = await _retrospectiveRepository.GetByIdWithColumnsAsync(
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

        _retrospectiveRepository.Update(retrospective);

        return retrospectiveItem;
    }
}