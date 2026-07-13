using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Infrastructure.Authentication;

/// <summary>
/// Maps a SignalR connection to the same user identifier <see cref="ClaimsPrincipalExtensions.GetUserId"/>
/// reads for HTTP requests, so hubs can target a specific user via <c>Clients.User(...)</c>.
/// </summary>
internal sealed class UserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    }
}
