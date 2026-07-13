## Problem Statement

Users have no way to learn about activity relevant to them without actively going and looking for it. Mentioning a teammate with `@` already works end-to-end in the comment/description editor - the mention chip renders, the suggestion list resolves project members - but nothing happens for the mentioned person; they only find out if they happen to reopen that work item or wiki page. The same is true for a work item assigned to you changing underneath you (status, sprint, assignee, priority, or anything else), someone commenting on your assigned work item, or being added to a project, team, or organization (the last of these already sends an email, but nothing shows up inside the product itself). Anyone who isn't actively re-checking every work item, wiki page, and project they care about will miss things that matter to them.

## Solution

An in-app Notification Center: a bell icon with an unread-count badge, opening a panel of notifications ordered newest-first. A notification is created when someone mentions you in a work item or wiki page comment, when a work item assigned to you changes (any field, one notification per save rather than one per field), when someone comments on a work item assigned to you, or when you're added to a project, team, or organization. New notifications arrive live via a real-time push, no refresh required. Clicking a notification navigates to what it's about and marks it read; there's also an explicit "mark all as read." Each of the four notification categories can be muted independently per user, on by default. Notifications are not emailed and never expire.

## User Stories

1. As a project member, I want to be notified when someone `@mentions` me in a work item comment, so that I know I've been asked to weigh in without having to watch every work item.
2. As a project member, I want to be notified when someone `@mentions` me in a wiki page comment, so that I don't miss being pulled into a documentation discussion.
3. As a project member, I want to be notified when a work item assigned to me changes - status, sprint, assignee, priority, story points, value area, parent, tags, links, attachments, or pull request links - so that I stay aware of what's happening on my own work without watching it constantly.
4. As a project member, I want all the fields changed in a single save to be grouped into one notification, so that a multi-field edit doesn't flood my notification list with one entry per field.
5. As a project member, I want to be notified when someone comments on a work item assigned to me, even if they didn't `@mention` me, so that I don't miss discussion happening on my own work.
6. As a project member, I want to be notified when I'm added to a project, team, or organization, so that I find out inside the product itself, not only by email.
7. As a project member, I do not want to be notified about my own actions - assigning myself, changing my own assigned work item, mentioning myself, or commenting on my own assigned work item - so that my notification list only contains things I actually need to look at.
8. As a project member, I do not want to be notified when a background integration (e.g. the GitHub pull request sync) changes a work item assigned to me, so that automated bulk syncs don't flood my notifications.
9. As a project member, I want each notification's message to keep reading correctly even if the person who triggered it later renames themselves or leaves the project, so that old notifications don't become confusing or broken.
10. As a project member, I want clicking a notification to take me directly to what it's about (the work item, comment, wiki page, or project/team/organization), so that I don't have to hunt for the context myself.
11. As a project member, I want clicking a notification to mark it as read, so that I don't have to separately dismiss things I've already looked at.
12. As a project member, I want an explicit "mark all as read" action, so that I can clear my list in one step without opening every item.
13. As a project member, I do not want merely opening the notification panel to mark everything read, so that unread items don't disappear before I've actually looked at them.
14. As a project member, I want new notifications to appear in real time without refreshing the page, so that the unread badge and list stay current while I'm working.
15. As a project member, I want to see an unread count on the bell icon, so that I know at a glance whether there's anything new.
16. As a project member, I want to independently mute mentions, assigned work item changes, comments on my assigned work items, and membership notifications, so that I can quiet the categories I don't find useful without losing the others.
17. As a project member, I want notification preferences to default to "on" for every category, so that I don't miss anything until I actively choose to mute it.
18. As a project member, I want my notification list to be paginated, so that a long history still loads quickly.
19. As a project member, I want a notification whose underlying work item, comment, or wiki page has been deleted to be removed along with it, so that I'm not left with dead links in my notification list.
20. As a project member, I do not want notifications to ever be automatically deleted or expired, so that I can still scroll back through older ones if I need to.
21. As a project member, I do not expect `@mentions` typed into a work item's Description, Acceptance Criteria, or a wiki page's body content to notify anyone yet, so that I'm not confused if that specific case behaves differently from comments.
22. As a project member, I do not expect any of these four notification types to also arrive by email, so that I'm not surprised by a new email channel opening up alongside the existing "added to project/organization" emails.
23. As a developer integrating with the API, I want a dedicated endpoint to fetch my paginated notification list, so that I can build the bell/panel UI (or alternate tooling) against it independently of other resources.
24. As a developer integrating with the API, I want dedicated endpoints to mark a single notification read and to mark all notifications read, so that read-state changes are explicit, separate actions.
25. As a developer integrating with the API, I want a dedicated endpoint to read and update my notification preferences, so that the mute toggles can be built as their own settings surface.
26. As a project member, I want to connect to a real-time channel that only ever pushes notifications addressed to me, so that I never see another user's notifications.

## Implementation Decisions

**Domain model** (new `Domain.Notifications`):
- `Notification`: `RecipientUserId`, a `NotificationType`, an `EntityId` (`Guid`, meaning dependent on `NotificationType`) for click-through navigation, a `Message` string resolved and stored at creation time (same reasoning as `WorkItemChange.OldValue`/`NewValue` in `docs/adr/0007-workitem-change-history.md`: must keep reading correctly after the triggering actor renames themselves or leaves), and a nullable `ReadAtUtc`.
- `NotificationType`: one value per concrete surface, not a generic scope enum - `MentionedInWorkItemComment`, `MentionedInWikiPageComment`, `AssignedWorkItemChanged`, `WorkItemCommentAdded`, `AddedToProject`, `AddedToTeam`, `AddedToOrganization`.
- `NotificationPreference`: one row per user with a boolean per notification *category* (Mentions, Assigned Work Item Changes, Work Item Comments, Membership - the three `AddedTo*` types share the Membership flag), defaulting to enabled.
- `Notification` cascade-deletes with its source entity (work item, comment, wiki page) when that's deleted, matching how `WorkItemChangeSet` behaves today. No retention/expiry policy, consistent with the rest of the codebase having no soft-delete or audit-retention precedent.
- Full rationale and rejected alternatives recorded in `docs/adr/0008-notification-center.md`.

**Capture mechanism** (`Infrastructure.Persistence`):
- A new `SaveChanges` interceptor, registered immediately after `WorkItemChangeHistoryInterceptor` (which is already near its 300-line class limit, so this is a new sibling class rather than an extension of it). It:
  - Reads the `WorkItemChangeSet` entries `WorkItemChangeHistoryInterceptor` just staged in the `ChangeTracker` this save, reusing their already-resolved display strings to build one `AssignedWorkItemChanged` notification per changeset for the work item's assignee (skipped when the changeset's `ChangedByUserId` is null - a system-actor/background-job change - or equals the assignee).
  - Reads newly-added `WorkItemComment`/`WikiPageComment` entries, parsing mention nodes (`data-type="mention" data-id="{userId}"`) out of the saved HTML to create `MentionedInWorkItemComment`/`MentionedInWikiPageComment` notifications, and separately creates a `WorkItemCommentAdded` notification for the work item's assignee (skipped when the assignee posted the comment themselves, or is also the mentioned user - no duplicate notification for the same comment).
  - Checks the recipient's `NotificationPreference` before staging each `Notification`, so muted categories never produce a row.
  - Stages each created `Notification` directly onto the same unit of work (same pattern as `WorkItemChangeSet` itself), and raises a lightweight domain event on it so the existing `DomainEventPublishingInterceptor` (which already runs after this point in the registered interceptor order) dispatches it before commit, matching how `UserAddedToProjectDomainEvent`'s email already fires pre-commit today - not a new risk this feature introduces.
- Membership notifications (`AddedToProject`, `AddedToTeam`, `AddedToOrganization`) are created by new `INotificationHandler`s on the *existing* `UserAddedToProjectDomainEvent`, `UserAddedToTeamDomainEvent`, `UserAddedToOrganizationDomainEvent`, added alongside (not replacing) the existing email handlers. These handlers also check `NotificationPreference` and raise the same lightweight domain event to trigger the real-time push.

**Real-time push** (`Application.Abstractions` / `Infrastructure` / `Presentation`):
- A new `INotificationRealtimeNotifier` abstraction (Application layer, Dependency Inversion Principle, mirroring `IGitHubIntegrationNotifier`), implemented in `Presentation.Hubs` via a new `NotificationHub`/`INotificationHub` pair (mirroring `GitHubIntegrationHub`).
- Unlike `RetrospectiveHub`/`GitHubIntegrationHub` (both broadcast to `Clients.All`), this hub pushes only to the recipient via `Clients.User(...)`, which requires a new `IUserIdProvider` mapping SignalR connections to the same `UserId` claim `IUserContext` already reads - this is new plumbing this feature needs, not present anywhere in the codebase today.
- The push payload is the full rendered `Notification` (not a thin refetch signal like `GitHubIntegrationHub`'s pattern), since the message is already fully resolved server-side and pushing it directly avoids a round trip.

**Application layer** (new `Application.Notifications`):
- `GetNotificationsQuery` - paginated (`PaginatedList<T>` convention, as used by `GetWorkItemHistoryQuery`), with an `unreadOnly` filter, ordered newest-first.
- `MarkNotificationAsReadCommand`, `MarkAllNotificationsAsReadCommand`.
- `GetNotificationPreferencesQuery`, `UpdateNotificationPreferencesCommand`.

**API contract** (new `Presentation.Controllers.Notifications`):
- `GET api/notifications` (paginated, `unreadOnly` query parameter) - top-level resource scoped implicitly to the authenticated user via `IUserContext`, matching the existing `api/users` convention rather than being nested under a project/organization.
- `POST api/notifications/{notificationId}/mark-read`
- `POST api/notifications/mark-all-read`
- `GET api/notifications/preferences`, `PUT api/notifications/preferences`
- New SignalR hub at `/hubs/notifications`.

**Frontend** (`mirai-react-app`):
- A bell icon in the main navigation with an unread-count badge, opening a dropdown/panel listing notifications (following this repo's established `api/ → queries/ → components` layering).
- A SignalR client hook (mirroring the existing GitHub-integration hub client) subscribing to `/hubs/notifications`, merging pushed notifications into the query cache and incrementing the unread badge live.
- Clicking a notification navigates to its target (derived from `NotificationType` + `EntityId`) and calls the mark-read mutation; a separate "mark all as read" control calls the bulk mutation.
- A notification preferences section (likely within existing account/settings UI) with one toggle per category, backed by the preferences endpoints.

## Testing Decisions

Tests assert observable behavior (persisted notification data, handler-to-notifier calls) rather than internal mechanics (`ChangeTracker` state, interceptor internals).

**`Application.IntegrationTests`** (primary seam, following the same convention as `WorkItems/Commands/LinkWorkItemsTests.cs` and the `work-item-history.md` spec): inherit `BaseIntegrationTest`, seed via `_dbContext`, execute real commands through the real `ISender` - `UpdateWorkItemCommand` for assignment/field changes, `AddCommentCommand`/`UpdateCommentCommand` (work item and wiki page) for comments and mentions, and the existing add-user-to-project/team/organization commands for membership - then send `GetNotificationsQuery` (or read `_dbContext` directly) and assert the resulting `Notification` rows: correct type, message, recipient, grouping (one per changeset), self-suppression, system-actor suppression, and preference-muting. `MarkNotificationAsReadCommand`/`MarkAllNotificationsAsReadCommand` tested the same way, asserting `ReadAtUtc` transitions.

**`Application.UnitTests`** (new, second seam, scoped only to the real-time push): a test on the new domain-event handler that calls `INotificationRealtimeNotifier`, with the notifier mocked via NSubstitute (matching this project's existing mocking convention), asserting it's invoked with the correct `Notification` payload for the correct recipient. This tests the handler's logic, not the SignalR transport itself - no live `TestServer`/SignalR-client test is introduced, consistent with there being no such test anywhere else in this codebase today.

**Frontend** - standalone component tests for the new notification panel/bell and preferences UI, following the exact pattern used for other feature areas (e.g. `work-item-history.test.tsx`): `renderWithProviders`, MSW-mocked HTTP responses, asserting rendered content and mark-read/navigate interactions rather than implementation details.

## Out of Scope

- Notifications for `@mentions` typed into a work item's Description, Acceptance Criteria, or a wiki page's body content - the editor already supports mentions there, but those fields are edited in place with commit-on-blur and have no discrete "authored" moment the way a comment does; this is an intentional v1 cut, not an oversight.
- Email delivery for any of the four new trigger types. The existing "Added to Project/Organization" emails are untouched and independent of this feature.
- Any retention, expiry, or deletion policy for notifications beyond cascading with their source entity.
- Per-project/per-team/per-organization granular membership notification toggles - the three `AddedTo*` types share a single "Membership" preference flag.
- Auto-marking notifications as read simply by opening the panel.
- Live/transport-level test coverage of the SignalR push (no `TestServer` + SignalR client test).
- A "watchers"/subscription concept beyond assignment (e.g. following a work item you're not assigned to). Assignment is the only membership-to-a-work-item concept this feature reacts to.

## Further Notes

- This spec assumes `docs/adr/0008-notification-center.md` as the source of truth for the underlying architectural decisions (fan-out mechanism, notification shape, real-time push design) and the rejected alternatives behind each.
- `CONTEXT.md` at the repo root now defines `Notification`, `Mention`, and `Notification Preference` - use this vocabulary consistently in code, comments, and API naming.
- Mention *authoring* (the `@` suggestion UI, the mention chip, HTML mention nodes) already exists in `mirai-react-app` today via `MentionableEditor`/`createMentionExtension`; this feature is purely about consuming what's already being written, not building new editor UI.
