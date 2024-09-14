using Mirai.Application.Common.Interfaces;
using Mirai.Application.Common.Models;

namespace TestCommon.Security;

public class TestCurrentUserProvider : ICurrentUserProvider
{
    private CurrentUser? _currentUser;

    public void Returns(CurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public CurrentUser GetCurrentUser() => _currentUser ?? CurrentUserFactory.CreateCurrentUser();
}
