// using System.Transactions;
using MediatR;
using Mirai.Application.Common.Interfaces;

namespace Mirai.Application.Common.Behaviors;

public sealed class UnitOfWorkBehavior<TRequest, TResponse>(IUnitOfWork _unitOfWork)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (IsNotCommand())
        {
            return await next();
        }

        // TODO: Verify how to use transaction scope
        // using var transactionScope = new TransactionScope();
        var response = await next();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // transactionScope.Complete();
        return response;
    }

    private static bool IsNotCommand()
    {
        return !typeof(TRequest).Name.EndsWith("Command");
    }
}