using Mirai.Application.Common.Models;

namespace Mirai.Application.Common.Interfaces;

public interface ICurrentUserProvider
{
    CurrentUser GetCurrentUser();
}