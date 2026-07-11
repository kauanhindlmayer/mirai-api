# 7. Work item change history

Date: 2026-07-10

## Status

Accepted

## Context

Work items have no history/timeline today - there is no way to see what changed on a work item or when, beyond the single `UpdatedAtUtc` timestamp on the aggregate.

We evaluated the history models of Azure DevOps ("revisions": a full snapshot of the work item stored on every save, diffed on read) and Jira ("changelog": field-level change records stored per save, with no full snapshot) as reference points.

## Decision

We will implement a Jira-style field-level changelog, not an Azure DevOps-style snapshot model:

- `WorkItemChangeSet` groups every change made in a single save (shared actor + timestamp), containing one or more `WorkItemChange` records.
- `WorkItemChange` covers both scalar field changes (Status, Title, AssigneeId, ...) and structural changes (tag/link/attachment/PR-link added or removed).
- Detection is automatic: an EF Core `SaveChanges` interceptor diffs the `ChangeTracker` (modified scalar properties plus added/removed child entities) rather than requiring each command handler or domain method to raise a dedicated event per field. New fields on `WorkItem` are captured without additional code.
- `WorkItemChange.OldValue`/`NewValue` are always stored as display strings, resolved at write time (e.g. a sprint's name, not its ID), so history reads correctly even after the referenced entity is renamed or deleted later. This means the interceptor performs lookups against related data (Sprint, User, Team, WorkItem names) during `SaveChangesAsync`, not a pure synchronous property diff.
- `WorkItemChangeSet.ChangedByUserId` is nullable. When a change originates from a background job or integration (e.g. the GitHub webhook sync jobs in `src/Infrastructure/Jobs/`) rather than a signed-in user, `ChangedByUserId` is null and a `SystemActor` label supplied by that integration is stored instead (e.g. "GitHub Integration") - there is no dedicated system `User` row.
- Comments remain a separate resource/feed, not part of the changelog, matching Jira's separation.
- History is not exposed over SignalR; it is fetched on demand (e.g. when a work item's detail view opens). No new real-time hub is introduced.
- History cascade-deletes with its `WorkItem`, consistent with how comments/attachments/links already behave on delete, and consistent with there being no soft-delete or audit-retention precedent elsewhere in the codebase.

## Alternatives Considered

- **Azure DevOps-style full snapshots per revision** - rejected: heavier storage growth, and still requires diffing snapshots at read time to answer "what changed," so it doesn't actually simplify anything over storing deltas directly.
- **Domain events per field** (extending the existing `PullRequestLinksUpdatedDomainEvent` pattern to every mutable field) - the more DDD-idiomatic option, but requires a domain method + event class per field and a rewrite of `UpdateWorkItemCommand`'s generic multi-field `Update(...)` into explicit per-field calls. Rejected in favor of the interceptor for now because it gives automatic coverage of every field (present and future) with far less code; this path remains open later for fields that need genuinely richer semantics than a generic diff.
- **Application-handler manual diffing** - rejected: duplicates "which fields matter" logic across every handler and is easy to miss when new commands are added.
- **Typed/discriminated value storage** (preserving original CLR types for `OldValue`/`NewValue`) - rejected as speculative; the primary consumer is a human-readable timeline, not analytics/filtering by historical value.
- **A dedicated system `User` row** for job-originated changes, keeping `ChangedByUserId` non-nullable - rejected because it pollutes people-facing UI (assignee pickers, member lists) with a fictitious user that must be filtered out everywhere.

## Consequences

- The `SaveChanges` interceptor needs access to reference data (Sprint, User, Team, WorkItem titles) to resolve display strings, so it cannot be a purely synchronous `ChangeTracker` diff - it runs as part of (or just before) `SaveChangesAsync` and may need targeted lookups for changed reference fields.
- Background jobs that mutate work items (`ProcessGitHubPullRequestWebhookJob`, `GitHubRepositorySyncJob`) need a way to supply a `SystemActor` label to the interceptor/ambient context, since they run outside any HTTP request/`IUserContext` scope.
- Deleting a work item permanently deletes its history - there is no audit trail surviving deletion, matching the rest of the codebase's lack of soft-delete/retention guarantees. If a compliance requirement for surviving audit trails emerges later, this decision should be revisited alongside introducing soft-delete generally.
