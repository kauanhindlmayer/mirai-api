# Completing a sprint carries over its unfinished work in one atomic act

Starting a sprint was already a deliberate act that fixes its committed scope
([ADR 0011](0011-sprint-lifecycle-is-stored-state.md)), but there was no way to
end one. A sprint could become `Active` and then stay that way forever: its
unfinished work was never triaged, and the one number a scrum master most wants
to see next to "what we committed to" - what we actually delivered - never got
recorded (SP-1).

`Team.CompleteSprint(sprintId, dispositions)` closes the active sprint. It
guards that the sprint is `Active`, requires a carry-over disposition for every
incomplete item, applies the moves, freezes the delivered figures, and sets the
sprint `Completed` with `CompletedAtUtc` - all in one call, so the aggregate is
saved in a single transaction.

## Completion is atomic, so it validates before it mutates

A completed sprint must never contain an un-triaged item. That is easy to state
and easy to get subtly wrong: if the moves were applied as each disposition was
read and a later one turned out to reference a sprint that is not `Planned`, the
sprint would already have half its items moved when the error was returned.

So `CompleteSprint` validates every carry-over target up front - each `Sprint`
target must name a `Planned` sibling - and only then applies any move. A
completion that will fail leaves the aggregate untouched. Coverage is checked the
same way: if any incomplete item lacks a disposition the whole call is rejected
before a single item moves.

## "Incomplete" means still in flight - Closed and Removed both stay

An item is offered for carry-over only when it is neither `Closed` nor `Removed`.
`Closed` is delivered and `Removed` is abandoned; both are terminal outcomes, and
both remain in the completed sprint as its permanent record. Carrying a cancelled
item forward into the next sprint would be nonsense, and the acceptance criteria
are explicit that delivered *and* removed items stay.

`Resolved` is deliberately **not** terminal here: it means dev-done-awaiting-
verification, so it is treated as unfinished and offered for carry-over. This is
the same line [ADR 0010](0010-closed-is-the-definition-of-delivered.md) draws for
velocity - `Closed` is the single definition of delivered - so the carry-over
dialog and the velocity chart can never disagree about what got done.

## Delivered figures compare like with like, and are frozen as columns

`CompletedStoryPoints` and `CompletedWorkItemCount` are computed over **committed
items only** - work added mid-sprint is scope change, not commitment delivery -
and at each item's **committed** points, not its current ones, so re-estimating a
work item after the fact cannot move a finished sprint's numbers. Only committed
items that are `Closed` count.

They are written to columns on the sprint at completion rather than recomputed on
read. Once the sprint is `Completed` its incomplete items have moved to other
sprints or the backlog, so the join that produced the figure no longer resolves
against the sprint's current membership - the answer has to be captured at the
moment it is still true. The committed figures stay derived from the immutable
`SprintCommitment` rows, which never move, so those remain a safe read.

## The lifecycle now gates every sprint operation

[ADR 0009](0009-sprint-editing-deletion-and-non-overlap.md) shipped edit and
delete with no lifecycle guard, and [ADR 0011](0011-sprint-lifecycle-is-stored-state.md)
deferred that guard to "the next slice". This is that slice, and it supersedes
both notes. The operations now gate on status:

| Operation | Planned | Active | Completed |
|---|---|---|---|
| Edit name/dates | full | name + end date only (start date locked) | rejected |
| Delete | allowed | rejected | rejected |
| Add work item | allowed | allowed (scope change) | rejected |
| Start | allowed | rejected | rejected |
| Complete | rejected | allowed | rejected |

An active sprint's start date is locked because it is the honest record of when
the sprint really began; only its name and end date may change, and the SP-2
non-overlap invariant still applies to any new end date. A completed sprint is
fully read-only. Delete is withheld the moment a sprint starts, because a sprint
with real history should not be casually destroyed - the exact reversal ADR 0009
predicted it would need once the lifecycle existed.

## Consequences

Completion does not start the next sprint. The team starts it deliberately, so
its commitment is snapshotted at the right moment rather than inheriting a
half-formed scope from the sprint that just closed. Lack of a next `Planned`
sprint therefore never blocks completing the current one: every incomplete item
simply goes to the backlog.

The delivered figures are the first sprint numbers Mirai freezes as historical
fact. This is only safe today because the system holds test data; on a system
with real history, changing how "delivered" is measured - as the `Closed`-only
switch did - would mean restating past sprints rather than silently recomputing
them.
