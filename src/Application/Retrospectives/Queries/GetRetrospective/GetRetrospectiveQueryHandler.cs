using Application.Common.Interfaces.Persistence;
using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Queries.GetRetrospective;

internal sealed class GetRetrospectiveQueryHandler(IRetrospectivesRepository _retrospectivesRepository)
    : IRequestHandler<GetRetrospectiveQuery, ErrorOr<Retrospective>>
{
    public async Task<ErrorOr<Retrospective>> Handle(
        GetRetrospectiveQuery query,
        CancellationToken cancellationToken)
    {
        var retrospective = await _retrospectivesRepository.GetByIdWithColumnsAsync(
            query.RetrospectiveId,
            cancellationToken);

        if (retrospective is null)
        {
            return RetrospectiveErrors.NotFound;
        }

        return retrospective;
    }
}