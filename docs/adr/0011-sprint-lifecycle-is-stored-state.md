# A sprint's lifecycle is stored state, not a date calculation

A sprint was a named date range and nothing else, so a team could describe a
sprint but not run one: nothing marked the sprint being worked in, and there was
no moment at which its committed scope could be fixed (SP-1).

A sprint now has a `Status` - `Planned`, `Active`, or `Completed` - stored on the
row, with `StartedAtUtc` recorded alongside it as audit. Starting is a deliberate
act performed by a person, not a consequence of the calendar reaching the start
date.

## Why not derive the state from the dates

Deriving "active" from `StartDate <= today <= EndDate` needs no column and can
never drift out of sync. It was rejected because it cannot express the things the
product needs:

- A sprint's committed scope is fixed *at the moment it starts*. If starting is
  something a date does rather than something a person does, there is no moment
  to hang that snapshot on.
- Sprints run late and start late. A team that plans on Tuesday for a sprint
  dated from Monday has an actively running sprint whose dates say otherwise.
- `Completed` is not a date fact at all. A sprint whose end date has passed but
  which nobody has closed is emphatically not completed - its work has not been
  triaged.

The dates remain the plan; the status is what is actually happening.

## One active sprint per team, enforced twice

`Team.StartSprint` refuses to start a sprint when a sibling is already `Active`,
because only the aggregate root can see its siblings. That guard cannot see a
*concurrent* starter, though: two requests can both read "no active sprint" and
both write. So a filtered unique index on `(team_id) WHERE status = 'Active'`
backs it, and the database refuses the second write. An integration test forces a
second sprint active past the domain and asserts the resulting Postgres unique
violation, since a unit test cannot see the index at all.

The filtered index is declared *alongside* the existing `team_id` lookup index
rather than replacing it. EF suppresses its convention-generated foreign-key index
as soon as any index is declared on the same column, and a partial index cannot
serve the unfiltered "sprints of this team" query that every sprint list runs -
so both are declared explicitly.

## Consequences

Existing sprints become `Planned` in the migration. None is marked `Active`:
there is no way to know which, if any, was really running, and guessing could
violate the new index. In practice this only affects test data.

Delete and edit are still not lifecycle-aware - a running sprint can still be
deleted, and a finished one edited. That is deliberate scope for the next slice;
this ADR establishes only the state and the transition into it. ADR 0009's note
that delete and edit carry no lifecycle guard therefore still stands, and will be
superseded once the gating lands.
