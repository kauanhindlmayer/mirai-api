# 5. Connect project to GitHub repository (GitHub App)

Date: 2026-07-06

## Status

Accepted

## Context

Mirai's project settings page already ships a disabled placeholder for this feature (`mirai-react-app/src/pages/projects/ProjectSettingsPage.tsx`, the "GitHub" tab).
The goal is to let a project be linked to a GitHub repository, then automatically (and manually, as a fallback) associate pull requests from that repository with the work items they reference, so a work item's detail view shows its linked PRs and their live status (open/merged/closed).

This is a new integration, not an extension of the existing "Sign in with GitHub" flow documented in ADR 0003.
That flow is brokered entirely through Keycloak as an Identity Provider and exists purely for user login.
It holds no credentials suitable for reading repository or PR data, and it has no webhook delivery.
Repo-level access needs its own GitHub App, kept fully separate from the login IdP, with its own credentials (App ID, private key, webhook secret) and its own `GitHubApp:*` configuration section distinct from the existing `GitHub:*` section used by `GitHubIdentityProviderOptions`.

## Decision

**GitHub App, not an OAuth App with broadened scope.**
A GitHub App gives fine-grained, repo-scoped permissions, installation-based tokens rather than a token tied to one user's personal access, and centrally configured webhook delivery.
This is the same model Jira, Linear, and Azure DevOps use for their own repo integrations.
The App is a brand-new registration, distinct from the OAuth App backing ADR 0003.

**Octokit.NET as the GitHub API client on the backend.**
It handles GitHub App JWT (RS256) authentication, installation-token exchange, PR and repo endpoints, pagination, and rate limits.
The RS256 App-JWT signing and installation-token lifecycle are nontrivial and security-sensitive to reimplement by hand.
`Octokit.Webhooks.AspNetCore` is used for the webhook receiver specifically because its `WebhookEventProcessor` base class and `MapGitHubWebhooks` endpoint mapping handle raw-body HMAC-SHA256 signature verification correctly, which is easy to get subtly wrong when implemented by hand (for example, verifying after the body has already been consumed by model binding).

**One GitHub repository per project for v1.**
This matches the literal feature wording (singular) and keeps the domain model simple: `GitHubRepositoryConnection` is a one-to-one child of `Project`.
It is modeled so it could extend to multiple repositories per project later, but that is not built now.

**The `#<code>` reference convention for automatic linking.**
`WorkItem.Code` is an `int`, unique only within its project.
Since a connected repository maps 1:1 to one project, a `#<code>` mention in a PR's title, body, or branch name is unambiguous once scoped to that project's repository, so no project-key prefix is needed.
This mirrors how the frontend already displays work items (`#{item.code}`).

**Both automatic (webhook) and manual linking.**
Webhook-driven detection on `pull_request` events is the primary mechanism.
A manual "Link pull request" UI, in the same shape as the existing "Link work item" affordance, covers PRs that don't reference a code, or repos where the webhook hasn't landed yet.
A recurring Quartz job (`GitHubRepositorySyncJob`) re-checks open pull request links and does a best-effort scan for recently updated PRs, as a safety net over the webhook path rather than a replacement for it.

## Alternatives Considered

### Reusing or broadening the ADR-0003 OAuth App

- **Pros:** No new App registration; reuses existing credentials.
- **Cons:** Mixes the login flow's security blast radius with repository access, and an OAuth App's permissions are coarser than a GitHub App's.
Rejected to keep user authentication and repository/PR access as two independently revocable, independently scoped concerns.

### Hand-rolled `HttpClient` against the GitHub REST API

- **Pros:** No new dependency; full control over the request shape.
- **Cons:** Reinvents RS256 JWT signing, installation-token refresh and caching, pagination, and rate-limit handling, all of which Octokit.NET already provides and tests.
Rejected in favor of the well-maintained client library.

### Pure polling instead of webhooks

- **Pros:** Simpler initial implementation; no public endpoint required.
- **Cons:** Higher latency for status updates, and it does not scale well as the number of connected repositories grows.
Rejected as the primary mechanism, but kept as `GitHubRepositorySyncJob`, a fallback safety net over the webhook path.

## Consequences

### Positive

- Repository and PR access is fully decoupled from user authentication: revoking the GitHub App installation never affects anyone's ability to log in, and vice versa.
- Octokit.NET and Octokit.Webhooks.AspNetCore absorb the security-sensitive parts (JWT signing, webhook signature verification), so Mirai's own code only deals with already-authenticated, already-verified data.
- The domain model (`GitHubRepositoryConnection`, `WorkItemPullRequestLink`) follows the same `Entity`/`ErrorOr`/aggregate-method conventions already used throughout the codebase, so it required no new architectural patterns.

### Negative

- GitHub webhooks require a publicly reachable HTTPS URL, and Mirai currently has no deployed environment.
Until one exists, the webhook path can only be exercised by whoever runs a local tunnel (smee.io, per `docs/getting-started.md`), which means automatic linking is not meaningfully "live" for anyone else until Mirai is actually deployed somewhere.
- Registering the GitHub App itself (permissions, webhook URL, private key) is a manual, one-time, non-code step, following this repo's existing secrets convention for the ADR-0003 OAuth App.
- The fallback-sync job's "recently updated" scan only inspects PR titles (GitHub's Search API does not return the PR body), so a `#<code>` reference mentioned only in a PR's body is caught by the next real webhook delivery rather than by the sync job itself.

### Open Questions

These are flagged here rather than blocking implementation:

- Should "repository already connected elsewhere" reveal which project has it, or stay silent to avoid leaking cross-org or cross-project information?
- Is keeping PR links on disconnect (rather than marking them stale) the desired product behavior?
- If a project reconnects to a different repository, should its old PR links be flagged as referencing a now-unrelated repository?
