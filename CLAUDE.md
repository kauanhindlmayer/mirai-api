# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Mirai is a web-based project management tool built with .NET 9 and Clean Architecture principles. The solution uses .NET Aspire for cloud-native development and orchestration.

## Architecture

The codebase follows Clean Architecture with clear separation of concerns:

- **Domain** (`src/Domain/`): Core business entities, value objects, enums, and domain rules. Contains aggregates like WorkItems, Organizations, Teams, Projects, and Users.
- **Application** (`src/Application/`): Use cases, commands, queries, and application services using CQRS with MediatR. Contains business logic and orchestration.
- **Infrastructure** (`src/Infrastructure/`): Data access, external services, and infrastructure concerns. Implements repository patterns and integrations.
- **Presentation** (`src/Presentation/`): REST API controllers, authentication, and web API presentation layer.
- **AppHost** (`src/AppHost/`): .NET Aspire orchestration and service configuration.
- **ServiceDefaults** (`src/ServiceDefaults/`): Shared configuration and cross-cutting concerns.

## Key Technologies

- **.NET 9** with C# preview features
- **Entity Framework Core** with PostgreSQL and pgvector for vector search
- **MediatR** for CQRS pattern implementation
- **FluentValidation** for input validation
- **SignalR** for real-time communication
- **Redis** for caching
- **Azure Blob Storage** for file storage
- **Keycloak** for authentication
- **OpenAI** for AI integration (chat and embeddings)
- **.NET Aspire** for orchestration
- **Quartz.NET** for background jobs

## Common Development Commands

### Building and Running
```bash
# Build the entire solution
dotnet build

# Run the application using Aspire orchestration
dotnet run --project src/AppHost

# Run the API directly (without Aspire)
dotnet run --project src/Presentation
```

### Testing
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Application.UnitTests
dotnet test tests/Application.IntegrationTests
dotnet test tests/Domain.UnitTests
dotnet test tests/ArchitectureTests
dotnet test tests/Presentation.FunctionalTests

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Database Operations
```bash
# Add new migration (run from src/Presentation)
cd src/Presentation
dotnet ef migrations add <MigrationName>

# Update database
dotnet ef database update

# Drop database
dotnet ef database drop
```

### Code Quality
```bash
# Check for StyleCop violations (built into build process)
dotnet build

# The project uses StyleCop.Analyzers with TreatWarningsAsErrors=true
```

### User Secrets Setup
```bash
# Run the setup script to configure user secrets
./scripts/setup-secrets.sh

# Or manually set secrets from src/Presentation:
cd src/Presentation
dotnet user-secrets set "Azure:BlobStorage:ConnectionString" "<value>"
dotnet user-secrets set "Keycloak:AuthClientSecret" "<value>"
```

### OpenAI Configuration

The application uses OpenAI for AI features (chat and embeddings) through .NET Aspire integration.

**Setup:**

Set the OpenAI API key in AppHost user secrets or as an environment variable:

```bash
cd src/AppHost
dotnet user-secrets set "Parameters:openai-openai-apikey" "sk-your-api-key-here"

# Or set as environment variable:
export OPENAI_API_KEY="sk-your-api-key-here"
```

**Default Models:**
- Chat: `gpt-4o-mini`
- Embeddings: `text-embedding-3-small`

These models are configured in [AppHost/Program.cs:21-24](src/AppHost/Program.cs#L21-L24) and can be modified as needed.

## Code Standards

**IMPORTANT**: Follow all coding standards defined in `.claude/rules/code-standards.md`. This includes naming conventions, code quality guidelines, function design principles, and code structure requirements.

## Test Standards

**IMPORTANT**: Follow all testing standards defined in `.claude/rules/test-standards.md`. This includes test organization, naming conventions, AAA pattern, factory usage, and coverage guidelines.

## Important Configuration

- **TreatWarningsAsErrors**: All projects treat warnings as errors for code quality
- **StyleCop.Analyzers**: Enforces code style and conventions
- **Nullable Reference Types**: Enabled across all projects
- **ImplicitUsings**: Enabled for cleaner code
- **LangVersion**: Set to preview to use latest C# features

## Testing Strategy

The solution includes comprehensive testing:
- **Unit Tests**: Fast, isolated tests for domain and application logic
- **Integration Tests**: Tests with real database and dependencies
- **Functional Tests**: End-to-end API testing
- **Architecture Tests**: Enforce architectural boundaries and dependencies

Test projects use xUnit v3, FluentAssertions, NSubstitute for mocking, and include code coverage collection.