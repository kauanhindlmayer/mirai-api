# 3. Add GitHub OAuth login

Date: 2026-07-04

## Status

Accepted

## Context

The frontend (`mirai-react-app`) already ships non-functional "Continue with GitHub" buttons on the login and signup forms. Today, all authentication is brokered through Keycloak using the OAuth2 Resource Owner Password Credentials (ROPC/"direct grant") flow: the API POSTs email+password directly to Keycloak's token endpoint via the confidential `mirai-auth-client`, and Keycloak-issued JWTs are validated by the API's JWT bearer middleware. The local Postgres `Users` table stores a lightweight profile keyed by `IdentityId` (the Keycloak `sub` claim), populated at registration time.

GitHub sign-in requires a redirect-based Authorization Code flow, which is architecturally different from the password grant: there is no password to submit, since the user authenticates on GitHub rather than in our UI.

## Decision

We keep the existing architecture intact: **the backend brokers the entire token exchange**, exactly like it already brokers the password grant. The frontend never talks to Keycloak directly for tokens — it only does a full-page redirect to Keycloak's authorize endpoint (with `kc_idp_hint=github`) and later hands the resulting `code` to a new API endpoint, `POST /api/users/login/github`.

The API exchanges the code for a Keycloak access token using `grant_type=authorization_code` against the same confidential `mirai-auth-client` and token endpoint already used for the password grant, decodes the token's claims (`sub`, `email`, `given_name`, `family_name`), and finds-or-creates the local `User`:

1. If a `User` exists with a matching `IdentityId`, it's an existing GitHub-linked login — update `LastActiveAtUtc` and return.
2. Else, if a `User` exists with a matching `Email` (registered via password originally), link the account by setting its `IdentityId`.
3. Else, create a new `User` and set its `IdentityId`.

GitHub itself is registered as a Keycloak **Identity Provider** on the `mirai` realm (Keycloak's "First Broker Login" flow), rather than the API talking to GitHub's OAuth API directly. This keeps GitHub as just another upstream identity source behind Keycloak, consistent with how this codebase already treats Keycloak as the single source of truth for authentication.

## Alternatives Considered

### Public/PKCE Keycloak client talking to the frontend directly

- **Pros**: Standard SPA OAuth pattern; no backend round-trip for the token exchange.
- **Cons**: Introduces a second, public Keycloak client alongside the existing confidential `mirai-auth-client`, and breaks the "API is the only thing that talks to Keycloak" invariant this codebase already relies on (`AuthenticationOptions`, `KeycloakOptions`, `JwtService`, `AuthenticationService`). Rejected to keep a single trust boundary.

### Frontend talks to GitHub's OAuth API directly, bypassing Keycloak

- **Pros**: Removes Keycloak as an intermediary for this specific flow.
- **Cons**: Would require a second identity system (GitHub-issued tokens) alongside Keycloak-issued JWTs, doubling the token validation and user-linking logic. Rejected in favor of routing GitHub through Keycloak's Identity Provider brokering, so the API only ever deals with one token format.

## Consequences

### Positive

- No new Keycloak client, no new token format for the API to validate — the existing `JwtService`/`AuthenticationService`/JWT bearer middleware setup is reused as-is.
- Local `User` provisioning (just-in-time creation, email-based linking) follows the same repository/unit-of-work patterns as `RegisterUser`/`LoginUser`.

### Negative

- Registering GitHub as a Keycloak Identity Provider requires a real GitHub OAuth App Client ID/Secret. Following this repo's secrets convention, this is a **manual, non-committed** step done once per environment via the Keycloak Admin Console (see `docs/getting-started.md`), rather than something `dotnet run --project src/AppHost` provisions automatically.
- For deployed environments, `src/AppHost/Program.cs` declares `GitHubClientId`/`GitHubClientSecret` parameters as a placeholder, but does not yet wire them to an automated Keycloak IdP provisioning step — that remains a manual Admin Console action today. Automating this (e.g. via the Keycloak Admin REST API at startup) is a follow-up, not part of this change.
