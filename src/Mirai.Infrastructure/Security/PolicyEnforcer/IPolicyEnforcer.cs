using ErrorOr;
using Mirai.Application.Common.Models;
using Mirai.Application.Common.Security.Request;

namespace Mirai.Infrastructure.Security.PolicyEnforcer;

public interface IPolicyEnforcer
{
    public ErrorOr<Success> Authorize<T>(
        IAuthorizeableRequest<T> request,
        CurrentUser currentUser,
        string policy);
}