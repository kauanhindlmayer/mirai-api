using Mirai.Domain.Retrospectives;

namespace Mirai.Application.Common.Interfaces;

public interface IRetrospectivesRepository
{
    Task AddAsync(Retrospective retrospective, CancellationToken cancellationToken);
}