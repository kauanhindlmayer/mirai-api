# Mirai

A web-based project management tool (work items, sprints, boards, wiki pages, retrospectives) built with Clean Architecture.

## Language

**Notification**:
An in-app record informing a user that something relevant to them occurred - they were mentioned, a work item assigned to them changed, someone commented on their assigned work item, or they were added to a project, team, or organization. Stores a display message resolved at the moment it's created, so it reads correctly even if the underlying names later change.
_Avoid_: Alert, Message, Activity

**Mention**:
An explicit reference to a user, authored by typing `@` and selecting them from a suggestion list while writing a work item comment or wiki page comment. Distinct from a work item being *assigned* to someone.
_Avoid_: Tag (already used for work item categorization), Ping

**Notification Preference**:
A per-user, per-trigger-type setting controlling whether a given kind of event produces a Notification for that user. Enabled by default; the user opts out, not in.
_Avoid_: Setting, Subscription
