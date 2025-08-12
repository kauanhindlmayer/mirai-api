using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateRetrospectiveColumn;

internal sealed class CreateRetrospectiveColumnCommandHandler
    : IRequestHandler<CreateRetrospectiveColumnCommand, ErrorOr<Guid>>
{
    private readonly IRetrospectiveRepository _retrospectiveRepository;

    public CreateRetrospectiveColumnCommandHandler(
        IRetrospectiveRepository retrospectiveRepository)
    {
        _retrospectiveRepository = retrospectiveRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateRetrospectiveColumnCommand command,
        CancellationToken cancellationToken)
    {
        var retrospective = await _retrospectiveRepository.GetByIdWithColumnsAsync(
            command.RetrospectiveId,
            cancellationToken);

        if (retrospective is null)
        {
            return RetrospectiveErrors.NotFound;
        }

        var column = new RetrospectiveColumn(command.Title, retrospective.Id);

        var result = retrospective.AddColumn(column);
        if (result.IsError)
        {
            return result.Errors;
        }

        _retrospectiveRepository.Update(retrospective);

        return column.Id;
    }
}