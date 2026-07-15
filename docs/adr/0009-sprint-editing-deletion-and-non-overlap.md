# Sprint editing, deletion, and non-overlap

Sprints could be created but never changed: `Sprint` had no way to alter its own
name or dates, and there were no update or delete endpoints, so a sprint created
with the wrong dates was permanent (backlog SP-2).
We added `PUT` and `DELETE /api/teams/{teamId}/sprints/{sprintId}`, and in doing
so had to decide what "delete" and "valid dates" actually mean for a sprint.

## Deleting a sprint returns its work items to the backlog

Deletion is a hard delete.
`Team.DeleteSprint` detaches every work item in the sprint (`WorkItem.SprintId = null`)
and then removes the sprint.
No history of the sprint survives.

We chose this over a soft delete (`DeletedAt`, hidden from `ListSprints`) because
of what the backlog *is*: a work item is in the backlog precisely when it belongs
to no sprint.
A soft-deleted sprint would leave its work items pointing at a sprint nobody can
see, so they would be in neither the sprint nor the backlog - the data would
assert something no screen could show.
Detaching them makes "returned to the backlog" literally true, using the only
definition of backlog membership the system has.

The cost is real: deleting a sprint destroys the record that those items were
ever in it.
That is acceptable *today* only because Mirai has no sprint history to lose - no
committed scope, no completed state, no velocity.
When SP-1 adds a sprint lifecycle, delete should almost certainly be restricted
to sprints that have not started, and completed sprints should become
undeletable.
This ADR should be revisited then rather than treated as settling the question
for a system that has history worth keeping.

The delete path must load the sprint's work items eagerly
(`GetByIdWithSprintsAndWorkItemsAsync`).
The `WorkItem` -> `Sprint` foreign key is configured `OnDelete(DeleteBehavior.NoAction)`,
so a sprint deleted without its work items loaded does not orphan them - Postgres
rejects the delete outright with `23503`.
An integration test covers this; a unit test cannot, because an in-memory
collection will happily "delete" a sprint a real database would refuse.

## The backlog is now actually the work items in no sprint

Defining deletion this way exposed that `GetBacklogQuery` did not agree with it.
Given no `SprintId`, it returned *every* work item assigned to the team, sprint or
not (`!query.SprintId.HasValue || wi.SprintId == query.SprintId`), so a work item
sitting in a sprint was still listed in the team's backlog.
"Returned to the backlog" was therefore invisible in the product: the item had
never left it.

The filter is now `wi.SprintId == query.SprintId`, which - because EF Core folds a
null parameter into `IS NULL` - means "the given sprint's items", or, when no
sprint is given, "the items in no sprint".
This is a deliberate behaviour change to the backlog view: work items already
committed to a sprint no longer appear in it.

## Sprints in a team may not overlap in time

Two sprints in the same team may not cover the same day, and a sprint's date
range includes both endpoints - so a sprint ending on the 14th and one starting
on the 15th are back-to-back, but one starting on the 14th overlaps.
The rule lives on `Team` next to the existing name-uniqueness rule, and is
enforced by `AddSprint` **and** `UpdateSprint` alike, excluding the sprint being
checked so that re-saving a sprint against itself passes.

Enforcing it only on update would have been worse than not having it: you could
still create two overlapping sprints and would then find neither of them
editable - the exact trap SP-2 exists to remove.
So create is now stricter than it used to be.
This was cheap precisely because the system holds only test data; on a system with
real sprints it would have needed a migration and a backfill decision, and that
will be true the next time we tighten an invariant here.

## A sprint may start and end on the same day

`CreateSprintCommandValidator` required `EndDate > StartDate`, forbidding a
one-day sprint.
With inclusive endpoints that reads as an accident rather than a decision, so both
validators now require `EndDate >= StartDate`.
Update must never be stricter than create, or "fix the sprint I got wrong" breaks
for the sprints most likely to need fixing.

## What this does not do

`SprintResponse` gained a `WorkItemCount` so the client can warn how many work
items a delete will return to the backlog, but there is still no `GET` for a
single sprint, and the board and burndown remain team-scoped rather than
sprint-scoped (BD-3 and RP-1).
