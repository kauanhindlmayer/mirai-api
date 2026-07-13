# 05 — Notification preferences

**What to build:** A project member can independently mute each of the four notification categories - Mentions, Assigned Work Item Changes, Work Item Comments, and Membership (the three `AddedTo*` types share one flag) - so that a muted category never produces a notification for them, while everything stays on by default until they change it.

**Blocked by:** 04 (which transitively depends on 03 and 01) — needs every notification creation path to exist so this ticket can guard each of them.

**Status:** ready-for-agent

- [ ] A `NotificationPreference` entity/row exists per user with one boolean per category (Mentions, Assigned Work Item Changes, Work Item Comments, Membership), defaulting to enabled for every category and for every existing user (no migration should leave any user in a state where a category is unexpectedly muted).
- [ ] A query returns the current user's preferences; a command updates them.
- [ ] REST endpoints expose reading and updating the current user's preferences.
- [ ] Every notification creation path added in tickets 01, 03, and 04 (the three membership handlers, the assigned-work-item-change path, and the comment/mention path) checks the recipient's relevant preference before creating a `Notification` row - a muted category produces no row at all, not a suppressed-but-created one.
- [ ] Frontend: a settings section with one toggle per category, backed by the preferences endpoints.
- [ ] Backend integration tests cover: muting each category individually suppresses that category's notification creation while leaving the others unaffected, and a fresh user has every category enabled by default.
- [ ] Frontend component test covers: toggling a preference and the resulting API call.
