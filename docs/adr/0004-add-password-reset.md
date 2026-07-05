# 4. Add password reset

Date: 2026-07-05

## Status

Accepted

## Context

Mirai currently has no way for a user who forgets their password to regain access to their account. The login form (`mirai-react-app/src/components/auth/login-form.tsx`) already ships a "Forgot your password?" link, but it points at `href="#"` — dead UI waiting to be wired up.

All authentication is brokered through Keycloak (see `docs/adr/0003-add-github-oauth-login.md`): the API never stores password hashes itself, and the codebase relies on the invariant "the API is the only thing that talks to Keycloak" (`AuthenticationOptions`, `KeycloakOptions`, `JwtService`, `AuthenticationService`). Any password-reset design must preserve that invariant rather than exposing Keycloak to the frontend.

Keycloak ships a built-in `execute-actions-email` admin action that emails the user a link to a Keycloak-hosted "update password" page. That would be the least code to write, but it redirects users away from the frontend entirely, breaking the existing full-screen auth page pattern (`LoginPage`/`SignupPage`) and giving us no control over email branding or content.

## Decision

The API owns the reset token lifecycle and email, exactly the way it already owns registration/login, and only talks to Keycloak's Admin API as the final step to actually change the password:

1. A user submits their email on a new `/forgot-password` frontend page. The API generates a random token via `RandomNumberGenerator.GetHexString`, stores it (with a one-hour expiry) on the local `User` row, and emails a link containing the token back to a new `/reset-password` frontend page. To prevent user enumeration, this endpoint always returns success regardless of whether the email exists, and only sends an email when it does.
2. The user opens the link and sets a new password. The API validates the token and its expiry, then calls Keycloak's Admin API (`PUT /admin/realms/{realm}/users/{id}/reset-password`) to actually change the password — the same Admin API + `AuthenticationService` HTTP client already used to create Keycloak users at registration.

The frontend never talks to Keycloak directly, consistent with how GitHub login (ADR 0003) and the password grant already work.

## Alternatives Considered

### Keycloak's built-in `execute-actions-email` flow

- **Pros**: Minimal backend code — Keycloak handles token generation, expiry, email delivery, and the password-update form.
- **Cons**: Redirects the user to a Keycloak-hosted page for both the email content and the password-update form, breaking the frontend's full-screen auth page pattern and giving us no control over email branding. Rejected to keep the entire user-facing flow inside the React app, consistent with how login and registration already work.

### Public/PKCE Keycloak client talking to the frontend directly

- **Pros**: Standard SPA pattern for account management flows.
- **Cons**: Introduces a second, public Keycloak client alongside the existing confidential `mirai-auth-client`, and breaks the "API is the only thing that talks to Keycloak" invariant this codebase already relies on. Rejected for the same reason ADR 0003 rejected it for GitHub login.

## Consequences

### Positive

- No new Keycloak client or token format — the existing `AuthenticationService`/Admin API HTTP client is reused as-is, only adding a `ResetPasswordAsync` method.
- The reset token lives on the local `User` row and follows the same repository/unit-of-work patterns as the rest of the `Users` aggregate, so it's covered by the existing persistence and testing conventions.
- The frontend keeps full control over the reset flow's UI and copy, matching the login/signup pages instead of redirecting to a Keycloak-hosted page.

### Negative

- The API now owns a second copy of password complexity rules (already duplicated once between `RegisterUserCommandValidator` and `ResetPasswordCommandValidator`), and a token lifecycle (generation, expiry, single-use invalidation) that Keycloak's built-in flow would have handled for free.
- Password reset emails go through `IEmailService` (`Infrastructure/Email/EmailService.cs`), which sends over SMTP to whatever `ConnectionStrings:mailpit` points at (the AppHost wires this to a local [Mailpit](https://mailpit.axllent.org/) container — see `docs/getting-started.md`) and otherwise just logs, same as the two other existing call sites. Wiring a real production email provider remains a separate, not-yet-scheduled piece of work.
