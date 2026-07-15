# Mirai

A web-based project management tool (work items, sprints, boards, wiki pages, retrospectives) built with Clean Architecture.

## Language

**Sprint**:
A named, time-boxed iteration owned by a single team, spanning an inclusive range of calendar dates (a sprint dated the 1st to the 14th includes the 14th). Within its team, a sprint's name is unique and its dates may not overlap another sprint's. Deleting a sprint returns its work items to the **Backlog**.
_Avoid_: Iteration, Cycle, Milestone

**Backlog**:
The work items belonging to no sprint. Backlog membership is not a flag or a separate list - a work item is in the backlog precisely when its `SprintId` is null, which is why deleting a sprint puts its work items back there.
_Avoid_: Icebox, Queue, Unplanned work

**Notification**:
An in-app record informing a user that something relevant to them occurred - they were mentioned, a work item assigned to them changed, someone commented on their assigned work item, or they were added to a project, team, or organization. Stores a display message resolved at the moment it's created, so it reads correctly even if the underlying names later change.
_Avoid_: Alert, Message, Activity

**Mention**:
An explicit reference to a user, authored by typing `@` and selecting them from a suggestion list while writing a work item comment or wiki page comment. Distinct from a work item being *assigned* to someone.
_Avoid_: Tag (already used for work item categorization), Ping

**Notification Preference**:
A per-user, per-trigger-type setting controlling whether a given kind of event produces a Notification for that user. Enabled by default; the user opts out, not in.
_Avoid_: Setting, Subscription

**Profile**:
A view of a user as seen from within one organization: their identity (name, email, avatar) plus the teams and projects they belong to *within that organization*. A user has a different Profile per organization; a viewer never sees memberships in organizations they don't share with the target. Distinct from the user's own account, which they edit via `/api/users/profile`.
_Avoid_: Account, Member card
