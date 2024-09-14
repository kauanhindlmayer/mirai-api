using ErrorOr;
using Mirai.Application.Common.Security.Request;
using Mirai.Infrastructure.Security.CurrentUserProvider;

namespace Mirai.Infrastructure.Security.PolicyEnforcer;

public interface IPolicyEnforcer
{
    public ErrorOr<Success> Authorize<T>(
        IAuthorizeableRequest<T> request,
        CurrentUser currentUser,
        string policy);
}