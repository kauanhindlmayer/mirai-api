using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Retrospectives;

namespace Mirai.Application.Retrospectives.Commands.CreateRetrospective;

public class CreateRetrospectiveCommandHandler(IRetrospectivesRepository _retrospectivesRepository)
    : IRequestHandler<CreateRetrospectiveCommand, ErrorOr<Retrospective>>
{
    public async Task<ErrorOr<Retrospective>> Handle(
        CreateRetrospectiveCommand request,
        CancellationToken cancellationToken)
    {
        var retrospective = new Retrospective(
            request.Title,
            request.Description,
            request.ProjectId);
        await _retrospectivesRepository.AddAsync(retrospective, cancellationToken);
        return retrospective;
    }
}