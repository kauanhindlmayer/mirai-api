## Problem Statement

When something changes on a work item, whoever's looking at it later has no way to answer "what changed, and when?" beyond a single `UpdatedAtUtc` timestamp on the item itself. There's no way to see who moved a work item to a different sprint, who changed its status, who unassigned it, or when a linked pull request was attached - the only trace is the current state. Anyone investigating "why is this work item like this" or "when did this get reassigned" has no record to look at, unlike Azure DevOps' revision history or Jira's changelog/activity tab, which both give users this out of the box.

## Solution

Every save to a work item - whether a field edit, a tag/link/attachment/pull-request-link being added or removed - is automatically captured as a change history entry, with no extra action required from the user making the change. A collapsed "History" section at the bottom of the work item detail view lets anyone expand it to see a chronological, paginated timeline of every change: what changed, its old and new value, who made it (or which integration, if it was automated), and when.

## User Stories

1. As a project member, I want to see a chronological history of everything that changed on a work item, so that I can understand how it got to its current state without asking teammates.
2. As a project member, I want each history entry to show the old and new value of what changed, so that I can see exactly what was different before.
3. As a project member, I want to see who made each change, so that I know who to ask if I have questions about it.
4. As a project member, I want changes made in the same save (e.g. changing status and assignee together) grouped into a single history entry, so that the timeline reflects what actually happened rather than an arbitrary list of field-level noise.
5. As a project member, I want status changes recorded in the history, so that I can see when a work item moved between states.
6. As a project member, I want assignee changes recorded in the history, so that I can see who was previously responsible for a work item.
7. As a project member, I want sprint changes recorded in the history, so that I can see when a work item moved between sprints.
8. As a project member, I want parent work item changes recorded in the history, so that I can see when a work item was re-parented.
9. As a project member, I want title, description, and acceptance criteria edits recorded in the history, so that I can see how the definition of a work item evolved.
10. As a project member, I want story points and priority changes recorded in the history, so that I can see how estimation/prioritization evolved.
11. As a project member, I want classification (value area) changes recorded in the history, so that I can see when that categorization changed.
12. As a project member, I want assigned-team changes recorded in the history, so that I can see when responsibility moved between teams.
13. As a project member, I want tags being added or removed recorded in the history, so that I can see how a work item's tagging evolved.
14. As a project member, I want links to other work items being added or removed recorded in the history, so that I can see how relationships between work items evolved.
15. As a project member, I want attachments being added or removed recorded in the history, so that I can see what supporting files were provided and when.
16. As a project member, I want pull request links being added, updated, or removed recorded in the history, so that I can see when code was connected to this work item.
17. As a project member, I want history entries for reference fields (assignee, sprint, parent, team) to show a readable name rather than a raw ID, so that the timeline stays understandable even if I don't know internal IDs.
18. As a project member, I want history entries to keep showing the readable name that was true at the time of the change, so that a later rename or deletion of the referenced entity (e.g. a renamed sprint) doesn't retroactively change how past history reads.
19. As a project member, I want changes made by an automated integration (e.g. GitHub webhook sync) to be clearly labeled as coming from that integration rather than attributed to a random or missing user, so that I'm not confused about who "changed" something.
20. As a project member, I want the history to be paginated, so that work items with a long history still load quickly.
21. As a project member, I want the history section to be collapsed by default in the work item detail view, so that opening a work item stays fast and the view isn't cluttered by default.
22. As a project member, I want the history to load only when I expand the section, so that I don't pay the cost of fetching it for every work item I open.
23. As a project member, I want comments to remain in their own section separate from the history timeline, so that free-form discussion isn't mixed with structured change records.
24. As a project member, I want the history of a deleted work item to no longer be accessible, so that there's no dangling reference to information about something that no longer exists.
25. As a developer integrating with the API, I want a dedicated endpoint to fetch a work item's change history, so that I can build tooling or alternate UIs against it independently of the main work item response.

## Implementation Decisions

**Domain model** (`Domain.WorkItems`):
- `WorkItemChangeSet`: one per save (groups everything changed in a single command execution). Carries a timestamp, a nullable `ChangedByUserId`, and a nullable `SystemActor` label (populated instead of `ChangedByUserId` when the change originates from a background job/integration, e.g. the GitHub webhook sync jobs, rather than a signed-in user). Contains a collection of `WorkItemChange`.
- `WorkItemChange`: one per scalar field or structural change within a `WorkItemChangeSet`. Carries the changed field/relationship identifier, `OldValue`/`NewValue` (always stored as display strings - resolved at write time for reference fields, e.g. a sprint's name rather than its ID, so history reads correctly even after the referenced entity is renamed or deleted).
- Both are owned by `WorkItem` and cascade-delete with it, matching how comments/attachments/links already behave on work item deletion - no independent survival of history after its subject is deleted.
- Fields in scope for scalar tracking: `Title`, `Description`, `AcceptanceCriteria`, `Type`, `Status`, `AssigneeId`, `AssignedTeamId`, `ParentWorkItemId`, `SprintId`, `Planning` (StoryPoints, Priority), `Classification` (ValueArea). Internal/derived fields (`SearchVector`, `CreatedAtUtc`/`UpdatedAtUtc` themselves) are excluded from tracking.
- Structural changes in scope: tag added/removed, work item link added/removed, attachment added/removed, pull request link added/updated/removed.
- Comments are explicitly out of the change-tracking model; they remain their own resource/feed.
- This model, and the rationale for choosing it over a full-snapshot (Azure DevOps-style) approach or per-field domain events, is recorded in `docs/adr/0007-workitem-change-history.md`.

**Capture mechanism** (`Infrastructure.Persistence`):
- Extends the existing `ApplicationDbContext.SaveChangesAsync` override (which today only calls `UpdateTimestamps()` and dispatches domain events before calling `base.SaveChangesAsync`) with logic that inspects `ChangeTracker` for modified scalar properties and added/removed child entities on `WorkItem`, and builds/adds the corresponding `WorkItemChangeSet`/`WorkItemChange` entities to the same unit of work, so they persist atomically with the change they describe.
- Resolving display strings for reference fields (sprint name, user name, team name, parent work item title) requires lookups against related data during `SaveChangesAsync`, not a pure synchronous diff.
- Background jobs that mutate work items (the GitHub PR webhook and repository sync jobs) need a way to supply their `SystemActor` label to this mechanism, since they run outside any HTTP request/user-context scope.

**Application layer** (`Application.WorkItems`):
- A new `GetWorkItemHistoryQuery` (paginated, following the existing `PaginatedList<T>`/`PaginatedListAsync` convention already used by `ListWorkItemsQueryHandler`), returning `WorkItemChangeSet` entries (each with its nested `WorkItemChange` items) ordered newest-first.

**API contract** (`Presentation.Controllers.WorkItems`):
- A new `GET api/projects/{projectId}/work-items/{workItemId}/history` endpoint, paginated via the standard `page`/`pageSize` query parameters, returning `PaginatedList<WorkItemChangeSetResponse>`. This is the first paginated nested-resource endpoint for work items - unlike comments/tags/links/attachments (which are small, bounded, and embedded directly in the single `GET work-items/{id}` response), history can grow unboundedly over a work item's lifetime and needs independent pagination.
- No real-time push (SignalR) for history; it is fetched on demand only, consistent with `docs/adr/0007-workitem-change-history.md`.

**Frontend** (`mirai-react-app`):
- New `src/api/work-item-history.ts`, `src/queries/work-item-history.ts` (`useWorkItemHistoryQuery`, paginated) following this repo's established `api/ → queries/ → components` layering and `use<Entity>Query` naming convention.
- New `WorkItemHistory` component rendered inside `work-item-detail-dialog.tsx`, placed as a collapsed-by-default section below the existing "Created/Updated" footer. Expanding it triggers the first fetch via `useWorkItemHistoryQuery`; it is not fetched eagerly with the rest of the dialog's data, matching every other decision to keep this on-demand.
- Each timeline entry displays the actor (user name, or the `SystemActor` label styled distinctly for automated changes), a relative timestamp, and the list of changes within that entry (field/relationship name plus old → new value).

## Testing Decisions

Tests should assert observable behavior (what a caller/user can see - the returned history data, the rendered timeline) rather than internal mechanics (e.g. not asserting on `ChangeTracker` internals or interceptor implementation details directly).

**Backend** - `Application.IntegrationTests`, following the existing convention for this module (e.g. `WorkItems/Commands/LinkWorkItemsTests.cs`, `WorkItems/Commands/DeleteAttachmentTests.cs`): inherit `BaseIntegrationTest`, seed via `_dbContext`, execute real mutating commands (e.g. `UpdateWorkItemCommand`, `AddTagCommand`, `LinkWorkItemsCommand`, `UploadAttachmentCommand`) through the real `ISender`, then send the new `GetWorkItemHistoryQuery` the same way and assert the resulting `WorkItemChangeSet`/`WorkItemChange` data (old/new values, actor, grouping). This is the module's existing seam - there are no HTTP-level functional tests for work items today, so none are introduced by this feature.

**Frontend** - a standalone component test (`work-item-history.test.tsx`) on the new `WorkItemHistory` component, following the exact pattern already used for every other work-item sub-section (`work-item-comments.test.tsx`, `work-item-tags-editor.test.tsx`, etc.): `renderWithProviders`, MSW-mocked HTTP responses (`src/test/mocks/`), asserting rendered timeline content (actor, values, grouping) rather than implementation details.

## Out of Scope

- Real-time/live updates to the history timeline (no new SignalR hub).
- Preserving history after its work item is permanently deleted (history cascade-deletes with the work item).
- Comment edit history (comments remain untracked by this feature; they already have their own add/edit/delete commands and UI).
- Full-snapshot/point-in-time reconstruction of a work item's entire past state (only field-level deltas are stored, not full snapshots).
- Typed/queryable historical values (old/new values are always stored and displayed as strings; no analytics or filtering by historical value type).
- Filtering/searching the history timeline (e.g. by field, by actor, by date range) - only chronological pagination is in scope.
- A dedicated system `User` row for automated changes (system-originated changes use a `SystemActor` label instead).

## Further Notes

- This spec assumes `docs/adr/0007-workitem-change-history.md` as the source of truth for the underlying architectural decisions (field-level changelog vs. snapshot, interceptor-based capture vs. per-field domain events, actor model). Refer to it for the reasoning and rejected alternatives behind each choice.
- `CONTEXT.md` at the repo root now defines `WorkItemChangeSet`, `WorkItemChange`, and `System actor` - use this vocabulary consistently in code, comments, and API naming.
