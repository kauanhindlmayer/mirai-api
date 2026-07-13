# 02 — Real-time push

**What to build:** Notifications created in ticket 01 (and every trigger added after it) arrive live in an already-open notification panel, with no page refresh or re-fetch needed. This is the first per-user-private real-time channel in the codebase - the two existing SignalR hubs both broadcast to every connected client, which is wrong for notification data.

**Blocked by:** 01 — needs the Notification entity and the "notification created" domain event it raises.

**Status:** ready-for-agent

- [ ] A `IUserIdProvider` maps SignalR connections to the same `UserId` the rest of the app reads via `IUserContext` (the JWT `sub` claim), so that `Clients.User(...)` targeting works correctly.
- [ ] A new SignalR hub pushes to a single recipient only (`Clients.User(...)`), never broadcasts to all connected clients.
- [ ] A notifier abstraction lives in the Application layer (Dependency Inversion) and is implemented against the new hub in Infrastructure/Presentation, mirroring the existing `IGitHubIntegrationNotifier`/`GitHubIntegrationHub` pattern.
- [ ] A handler on the "notification created" domain event from ticket 01 pushes the full rendered `Notification` (not just an ID/refetch signal) to its recipient.
- [ ] Frontend: a SignalR client hook subscribes to the new hub and merges pushed notifications into the panel's data and the unread badge count live.
- [ ] Demo check: with two authenticated sessions open, one user being added to a project (ticket 01's trigger) appears in the other user's open notification panel without any manual refresh.
- [ ] A unit test (mocked notifier via NSubstitute, following this project's existing mocking convention) asserts the domain event handler calls the notifier with the correct notification and recipient. No live `TestServer`/SignalR-client test is required - this matches the current lack of any test coverage on the equivalent GitHub-integration notifier.
