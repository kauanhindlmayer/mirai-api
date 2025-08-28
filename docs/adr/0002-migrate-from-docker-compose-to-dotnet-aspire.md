# 2. Migrate from Docker Compose to .NET Aspire

Date: 2025-05-30

## Status

Accepted

## Context

Mirai currently uses Docker Compose to orchestrate its services. This setup has worked well for local development and basic deployments, but it introduces limitations as the system grows:

- **Service Discovery & Configuration**: Requires manual environment variable wiring and network configuration in Compose
- **Observability**: Logging, tracing, and metrics must be manually integrated, making distributed diagnostics harder
- **Scalability**: Compose is not designed for scaling or production-grade orchestration
- **Cloud-Native Alignment**: Modern .NET tooling provides built-in support for orchestrating services with observability and health checks

Our specific services orchestrated with Docker Compose include:

- PostgreSQL database with pgvector extension
- Redis cache
- Keycloak for authentication
- Ollama with LLM models
- Vue frontend application

.NET Aspire offers a modern approach to cloud-native application orchestration with better tooling integration and first-class support for distributed applications.

## Decision

We will migrate from Docker Compose to .NET Aspire for local development orchestration.

Key changes include:

- Define services as Aspire resources rather than Compose containers
- Use Aspire's service discovery instead of manual Compose networking
- Adopt Aspire's built-in observability stack (structured logging, metrics, tracing)
- Standardize health checks and readiness probes with Aspire conventions
- Retain Docker containers only where required (e.g., PostgreSQL) but manage them through Aspire's orchestration

## Alternatives Considered

### Keep Docker Compose

- **Pros**: Simple, already working, developer familiarity
- **Cons**: Lacks observability, resilience patterns, and deeper .NET integration

### Move directly to Kubernetes

- **Pros**: Production-grade orchestration, scalability
- **Cons**: Higher complexity for local development; Aspire provides a smoother developer experience while still being Kubernetes-compatible

### Use plain containers + manual orchestration

- **Pros**: Maximum flexibility
- **Cons**: Higher operational burden, no alignment with modern .NET ecosystem

## Consequences

### Positive

- **Better Developer Experience**: Integrated with Visual Studio/VS Code
- **Simplified Configuration**: Type-safe C# configuration instead of YAML
- **Service Discovery**: Built-in service discovery and communication
- **Health Checks**: Automatic health monitoring and dashboards
- **Hot Reload**: Better integration with .NET hot reload capabilities
- **Observability**: Built-in telemetry, logging, and metrics
- **Resource Management**: Easier management of external dependencies
- **Kubernetes-Ready**: Easier transition to Kubernetes in the future (Aspire supports exporting to K8s)

### Negative

- **Learning Curve**: Team needs to learn Aspire concepts and APIs
- **New Technology**: Aspire is relatively new (potential stability concerns)
- **Platform Lock-in**: More tightly coupled to .NET ecosystem
- **Migration Effort**: Need to rewrite existing Docker Compose configurations
