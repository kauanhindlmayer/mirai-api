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

"Continue with GitHub" is brokered entirely by Keycloak: the frontend redirects to Keycloak's authorize endpoint, Keycloak forwards the user to GitHub, and the API exchanges the resulting authorization code for a Keycloak access token. This requires registering GitHub as an Identity Provider on the `mirai` realm — a one-time manual step that is **not** committed to source control (the realm export's `identityProviders` array is intentionally left empty).

To enable it locally:

1. Register a new [GitHub OAuth App](https://github.com/settings/developers):
   - **Homepage URL**: `http://localhost:5173` (the frontend origin).
   - **Authorization callback URL**: `http://localhost:8080/realms/mirai/broker/github/endpoint` (Keycloak's local port is pinned to `8080` in `src/AppHost/Program.cs`).
2. Start the AppHost (`dotnet run --project ./src/AppHost/AppHost.csproj`) and open the Keycloak Admin Console for the `mirai` realm.
3. Under **Identity Providers**, add a **GitHub** provider with alias `github`, and enter the OAuth App's Client ID and Client Secret from step 1.
4. Click "Continue with GitHub" on the frontend's login/signup page to verify the flow end-to-end.

This configuration is persisted in the Keycloak container's data volume, so it only needs to be done once per environment.
