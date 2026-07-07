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

## Connect a project to a GitHub repository (pull request linking)

Linking pull requests to work items uses a separate GitHub App, distinct from the OAuth App used for "Sign in with GitHub" above.
It needs its own registration, its own secrets, and — because GitHub webhooks require a public HTTPS URL — a local tunnel, since Mirai has no deployed environment yet.

### 1. Register a GitHub App

Register a new [GitHub App](https://github.com/settings/apps/new) (not an OAuth App):

- **Homepage URL**: `http://localhost:5173`.
- **Setup URL** (under "Post installation"): `http://localhost:5173/auth/github/installation-callback`, and check "Redirect on update".
- **Webhook URL**: the smee.io channel URL from step 2 below — create that first, then come back and fill this in.
- **Webhook secret**: generate any random string and remember it.
- **Repository permissions**: `Pull requests: Read-only`, `Metadata: Read-only` (the latter is mandatory and auto-selected).
- **Subscribe to events**: `Pull request`, `Installation`, `Installation repositories`.
- **Where can this GitHub App be installed?**: "Only on this account" is fine for local testing.

After creating the App, note down:

- The **App ID**, shown at the top of the App's settings page.
- A **private key**: generate one and download the `.pem` file.
- The App's **slug**, taken from its public page URL (`https://github.com/apps/<slug>`).

You do **not** need the App's Client ID/Client Secret for this flow — Mirai only authenticates as the App via its JWT (App ID + private key) and never performs an OAuth code exchange for this integration.

### 2. Start a local webhook tunnel (smee.io)

GitHub webhooks must POST to a public HTTPS URL, even for local development, so this step isn't optional:

1. Go to [smee.io](https://smee.io/) and click "Start a new channel". Copy the channel URL, e.g. `https://smee.io/AbCdEfGh123`, and use it as the GitHub App's **Webhook URL** in step 1.
2. Run the smee client, forwarding to the API's local webhook endpoint:

```bash
npx smee-client --url https://smee.io/AbCdEfGh123 --path /api/webhooks/github --port 5001
```

(`5001` is the API's local `http` port from `src/Presentation/Properties/launchSettings.json` — check the Aspire Dashboard's `mirai-api` resource if it differs in your setup.)

Leave this running for the whole session; it's the only thing standing in for a real deployment right now.

### 3. Set the GitHub App's secrets

```bash
cd src/AppHost
dotnet user-secrets set "Parameters:GitHubAppId" "<app-id>"
dotnet user-secrets set "Parameters:GitHubAppInstallUrlSlug" "<app-slug>"
dotnet user-secrets set "Parameters:GitHubAppWebhookSecret" "<webhook-secret>"
dotnet user-secrets set "Parameters:GitHubAppPrivateKey" "$(cat /path/to/downloaded-key.pem)"
```

`GitHubAppClientId` is also prompted for by the AppHost (same as the other parameters) but isn't consumed by any code yet — submit it blank.

### 4. Try it end-to-end

1. Start the AppHost (`dotnet run --project ./src/AppHost/AppHost.csproj`) with the smee client from step 2 still running.
2. In the frontend, open a project's Settings page, go to the GitHub tab, and click "Connect your GitHub Account".
3. Complete GitHub's install flow and pick the repository you want linked (or let it auto-connect if the installation only has access to one).
4. Open a pull request against that repository titled something like `Fixes #<code>`, using a real work item code from that project.
5. Watch the smee terminal for the delivered webhook, then check the work item's detail view — the linked PR should appear (live, via SignalR, if the dialog was already open; otherwise on next load).
6. Close or merge the PR and confirm the status badge updates the same way.

## Viewing emails sent locally (Mailpit)

The API never talks to a real email provider locally. The AppHost runs [Mailpit](https://mailpit.axllent.org/), a local SMTP test server, and `EmailService` sends to it whenever it's reachable (falling back to logging only, as before, if it isn't — e.g. when running `dotnet run --project src/Presentation` directly without the AppHost).

To see an email the API sent (e.g. a password reset link), open Mailpit's web UI at `http://localhost:8025` — every message sent through `IEmailService.SendEmailAsync` shows up there instantly, with no real inbox or SMTP provider involved.
