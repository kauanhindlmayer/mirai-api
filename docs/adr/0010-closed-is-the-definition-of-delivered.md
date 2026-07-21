# Closed is the definition of delivered

Velocity counted a work item as delivered when its status was `Closed` **or**
`Resolved`. The sprint lifecycle work (SP-1) treats anything not `Closed` as
unfinished and offers it for carry-over into the next sprint, so keeping both
definitions would let one work item simultaneously score toward a sprint's
velocity *and* be rolled forward as unfinished - two parts of the product
disagreeing about what got done.

`Closed` is now the single, project-wide definition of **Delivered**. `Resolved`
means dev-done-awaiting-verification and scores nothing until it is closed.
Velocity, the sprint report, and carry-over therefore cannot contradict each
other.

## Consequences

This is a user-visible behaviour change: teams that parked work in `Resolved`
saw those points in their velocity and no longer will. It was safe to make now
only because the system holds test data; on a system with real history it would
have needed a conversation about restating past velocity.

Through the domain, the blast radius is narrow, and the reason is worth recording
because it is not obvious from the code. `CompletedAtUtc` is stamped only when an
item becomes `Closed`, and cleared only when it returns to `New`/`Active` - so a
`New -> Resolved` item never had a `CompletedAtUtc` and was *already* excluded by
the velocity query's `CompletedAtUtc.HasValue` filter. Via the domain, the only
items that stop counting are those closed and then moved back to `Resolved`,
which retain the timestamp.

The seeder is the exception, and it dominates in practice. It chose `Closed` or
`Resolved` at random as a "completed status" and then wrote `CompletedAtUtc` onto
the item directly by reflection, bypassing the domain rule - so roughly half of
all seeded completed work was `Resolved` *with* a timestamp, and did count toward
velocity. On a seeded database this change is therefore not narrow at all: that
half drops out. The seeder now completes work as `Closed` only, so it agrees with
the definition above rather than manufacturing data the domain could never
produce.

An integration test pins the transitions that decide this - `Closed`,
`New -> Resolved`, `Closed -> Resolved`, `Closed -> Removed`, and still-active -
so it cannot silently regress.

`Removed` work continues to count for nothing, and unestimated `Closed` work
continues to contribute zero points while still counting as a delivered item.

The predicate itself is spelled out at each site rather than shared: the chart
queries run as EF LINQ and must translate to SQL, so they cannot call a helper on
the domain. The definition therefore lives in three places that must agree -
`WorkItem`'s completion rule, the velocity query, and the cycle-time query - plus
the sprint report and carry-over as they arrive. They agree today; the test above
is what keeps velocity honest, and anything new that measures finished work should
gain its own test rather than trusting the duplication.
