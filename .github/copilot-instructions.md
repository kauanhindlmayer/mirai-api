# GitHub Copilot Instructions

## Project Overview

This is the Mirai API, a .NET 9 web-based project management tool built with Clean Architecture principles and .NET Aspire for cloud-native development.

## Architecture

Follow Clean Architecture with these layers:

- **Domain**: Core business entities and rules
- **Application**: Use cases with CQRS pattern using MediatR
- **Infrastructure**: Data access and external services
- **Presentation**: REST API controllers and web layer

## Code Standards

**IMPORTANT**: Follow all coding standards defined in `.claude/rules/code-standards.md`.

## Test Standards

**IMPORTANT**: Follow all testing standards defined in `.claude/rules/test-standards.md`.

## Key Technologies

- **.NET 9** with C# preview features and nullable reference types
- **Entity Framework Core** with PostgreSQL and pgvector
- **MediatR** for CQRS implementation
- **FluentValidation** for input validation
- **ErrorOr** for error handling
- **xUnit v3** with FluentAssertions and NSubstitute for testing
- **SignalR** for real-time communication
- **Redis** for caching

## Configuration

- TreatWarningsAsErrors is enabled
- StyleCop.Analyzers enforces code style
- Implicit usings are enabled
- Preview C# language features are available
