using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface ICurrentUserProvider
{
    CurrentUser GetCurrentUser();
}