using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Retrospectives;

namespace Mirai.Application.Retrospectives.Queries.GetRetrospective;

public class GetRetrospectiveQueryHandler(IRetrospectivesRepository _retrospectivesRepository)
    : IRequestHandler<GetRetrospectiveQuery, ErrorOr<Retrospective>>
{
    public async Task<ErrorOr<Retrospective>> Handle(
        GetRetrospectiveQuery query,
        CancellationToken cancellationToken)
    {
        var retrospective = await _retrospectivesRepository.GetByIdAsync(
            query.RetrospectiveId,
            cancellationToken);

        if (retrospective is null)
        {
            return RetrospectiveErrors.RetrospectiveNotFound;
        }

        return retrospective;
    }
}