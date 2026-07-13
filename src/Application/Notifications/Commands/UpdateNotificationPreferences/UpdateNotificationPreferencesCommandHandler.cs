using Application.Abstractions.Authentication;
using Domain.Notifications;
using ErrorOr;
using MediatR;

namespace Application.Notifications.Commands.UpdateNotificationPreferences;

internal sealed class UpdateNotificationPreferencesCommandHandler
    : IRequestHandler<UpdateNotificationPreferencesCommand, ErrorOr<Success>>
{
    private readonly INotificationPreferenceRepository _notificationPreferenceRepository;
    private readonly IUserContext _userContext;

    public UpdateNotificationPreferencesCommandHandler(
        INotificationPreferenceRepository notificationPreferenceRepository,
        IUserContext userContext)
    {
        _notificationPreferenceRepository = notificationPreferenceRepository;
        _userContext = userContext;
    }

    public async Task<ErrorOr<Success>> Handle(
        UpdateNotificationPreferencesCommand command,
        CancellationToken cancellationToken)
    {
        var preference = await _notificationPreferenceRepository.GetByUserIdAsync(
            _userContext.UserId,
            cancellationToken);

        if (preference is null)
        {
            preference = new NotificationPreference(_userContext.UserId);
            preference.Update(
                command.MentionsEnabled,
                command.AssignedWorkItemChangesEnabled,
                command.WorkItemCommentsEnabled,
                command.MembershipEnabled);
            await _notificationPreferenceRepository.AddAsync(preference, cancellationToken);

            return Result.Success;
        }

        preference.Update(
            command.MentionsEnabled,
            command.AssignedWorkItemChangesEnabled,
            command.WorkItemCommentsEnabled,
            command.MembershipEnabled);
        _notificationPreferenceRepository.Update(preference);

        return Result.Success;
    }
}
