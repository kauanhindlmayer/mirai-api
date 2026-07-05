# Getting Started

## Installation

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
- [Docker](https://docs.docker.com/get-started/get-docker/).
- [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd).

### Steps

1. Clone the repository:

```bash
$ git clone git@github.com:kauanhindlmayer/mirai-api.git
```

2. Enter the `mirai-api` directory:

```bash
$ cd mirai-api
```

3. Start the services with .NET Aspire:

```bash
dotnet run --project ./src/AppHost/AppHost.csproj
```

When you run the AppHost for the first time, the Aspire Dashboard will prompt you to enter the required secrets. These are automatically stored in user secrets and persisted for future runs.

## Sign in with GitHub

"Continue with GitHub" is brokered entirely by Keycloak: the frontend redirects to Keycloak's authorize endpoint, Keycloak forwards the user to GitHub, and the API exchanges the resulting authorization code for a Keycloak access token. This requires registering GitHub as an Identity Provider on the `mirai` realm. The API provisions and keeps this Identity Provider up to date automatically on startup — you only need to supply a GitHub OAuth App's credentials.

To enable it locally:

1. Register a new [GitHub OAuth App](https://github.com/settings/developers):
   - **Homepage URL**: `http://localhost:5173` (the frontend origin).
   - **Authorization callback URL**: `http://localhost:8080/realms/mirai/broker/github/endpoint` (Keycloak's local port is pinned to `8080` in `src/AppHost/Program.cs`).
2. Set the OAuth App's Client ID and Client Secret as AppHost user secrets:

```bash
cd src/AppHost
dotnet user-secrets set "Parameters:GitHubClientId" "<client-id>"
dotnet user-secrets set "Parameters:GitHubClientSecret" "<client-secret>"
```

3. Start the AppHost (`dotnet run --project ./src/AppHost/AppHost.csproj`). On startup, the API creates (or updates, if the secret was rotated) the `github` Identity Provider on the `mirai` realm via Keycloak's Admin REST API — no manual Admin Console step required.
4. Click "Continue with GitHub" on the frontend's login/signup page to verify the flow end-to-end.

If you don't want to set up GitHub sign-in locally, the AppHost will still prompt for `GitHubClientId`/`GitHubClientSecret` the first time you run it (same as the other required secrets) — just submit them blank. The API treats an empty value as "not configured" and skips provisioning entirely.

If GitHub sign-in stops working after previously working (e.g. "Unexpected error when authenticating with identity provider"), the most common cause is that the GitHub OAuth App's client secret was regenerated on GitHub's side without updating `Parameters:GitHubClientSecret` — update the secret and restart the AppHost so the API can push the new value to Keycloak.

## Viewing emails sent locally (Mailpit)

The API never talks to a real email provider locally. The AppHost runs [Mailpit](https://mailpit.axllent.org/), a local SMTP test server, and `EmailService` sends to it whenever it's reachable (falling back to logging only, as before, if it isn't — e.g. when running `dotnet run --project src/Presentation` directly without the AppHost).

To see an email the API sent (e.g. a password reset link), open Mailpit's web UI at `http://localhost:8025` — every message sent through `IEmailService.SendEmailAsync` shows up there instantly, with no real inbox or SMTP provider involved.
