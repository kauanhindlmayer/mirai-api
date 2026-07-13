# 03 — Assigned work item change notifications

**What to build:** A project member is notified when a work item assigned to them changes - any field the existing change history already tracks (status, sprint, assignee, priority, story points, value area, parent, tags, links, attachments, pull request links) - with every field changed in a single save grouped into one notification rather than one per field. Reuses ticket 01's Notification entity, persistence, API, and panel; if ticket 02 has landed, these arrive live for free through the pattern it already established.

**Blocked by:** 01 — reuses the Notification entity and creation/domain-event pattern it establishes. Not blocked by 02.

**Status:** ready-for-agent

- [ ] A new `NotificationType` (`AssignedWorkItemChanged`) is added, with `EntityId` pointing at the work item.
- [ ] A new EF Core `SaveChanges` interceptor - a sibling to `WorkItemChangeHistoryInterceptor` (which is already near its class-size limit), registered immediately after it - reads the `WorkItemChangeSet` that interceptor just staged this save, reusing its already-resolved field display strings rather than re-diffing.
- [ ] Exactly one notification is created per `WorkItemChangeSet` (i.e. per save), summarizing every field changed in it - not one notification per field.
- [ ] The work item's assignee is not notified when they are also the one who made the change (self-suppression).
- [ ] The work item's assignee is not notified when the change's `ChangedByUserId` is null - i.e. the change came from a background job/integration (e.g. GitHub pull request sync) rather than a signed-in human.
- [ ] A work item with no assignee produces no notification from this trigger.
- [ ] Backend integration tests (real `UpdateWorkItemCommand`/tag/link/attachment commands through `ISender`) cover: single-field change, multi-field change grouped into one notification, self-suppression, and system-actor suppression.
- [ ] No new frontend work is required beyond what ticket 01 already built - the existing panel renders this notification type using its stored message and navigates to the work item on click.
