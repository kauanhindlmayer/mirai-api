# Getting Started

## Installation

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0).
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
