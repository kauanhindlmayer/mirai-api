using System.Net.Mime;
using Application.Abstractions;
using Application.Notifications.Commands.MarkAllNotificationsAsRead;
using Application.Notifications.Commands.MarkNotificationAsRead;
using Application.Notifications.Queries.GetNotifications;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Constants;

namespace Presentation.Controllers.Notifications;

[ApiVersion(ApiVersions.V1)]
[Route("api/notifications")]
[Produces(MediaTypeNames.Application.Json, CustomMediaTypeNames.Application.JsonV1)]
public sealed class NotificationsController : ApiController
{
    private readonly ISender _sender;

    public NotificationsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Retrieve the current user's notifications.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<NotificationResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<NotificationResponse>>> GetNotifications(
        [FromQuery] NotificationsQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var query = new GetNotificationsQuery(
            parameters.UnreadOnly,
            parameters.Page,
            parameters.PageSize);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Mark a notification as read.
    /// </summary>
    /// <param name="notificationId">The notification's unique identifier.</param>
    [HttpPost("{notificationId:guid}/mark-read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> MarkNotificationAsRead(
        Guid notificationId,
        CancellationToken cancellationToken)
    {
        var command = new MarkNotificationAsReadCommand(notificationId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }

    /// <summary>
    /// Mark all of the current user's notifications as read.
    /// </summary>
    [HttpPost("mark-all-read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> MarkAllNotificationsAsRead(CancellationToken cancellationToken)
    {
        var command = new MarkAllNotificationsAsReadCommand();

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }
}
