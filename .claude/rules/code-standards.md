# Code Standards

## Naming Conventions

- All source code must be written in English
- Use PascalCase for:
  - Classes, interfaces, structs, enums
  - Methods and properties
  - Public fields and constants
  - Namespaces
  - Events and delegates
- Use camelCase for:
  - Private and internal fields
  - Local variables and parameters
  - Method arguments
- Use PascalCase for file names (matching the main type name)
- Use kebab-case for project and solution directories
- Prefix interfaces with `I` (e.g., `IUserService`)
- Use descriptive names for async methods ending with `Async`
- Use `_` prefix for private fields (e.g., `_context`, `_logger`)

## Code Quality

- Avoid abbreviations, but also don't write names longer than 30 characters
- Declare constants to represent magic numbers for readability
- Methods and functions should perform a clear and well-defined action, reflected in their name, which should start with a verb, never a noun

## Function Design

- Whenever possible, avoid passing more than 3 parameters; prefer using objects when necessary
- Avoid side effects; generally a method or function should either mutate or query, never allow a query to have side effects
- Never nest more than two if/else statements; always prefer early returns
- Never use flag parameters to switch method and function behavior; in these cases, extract to methods and functions with specific behaviors

## Code Structure

- Avoid long methods with more than 50 lines
- Avoid long classes with more than 300 lines
- Always invert dependencies for external resources in both use cases and interface adapters using the Dependency Inversion Principle

## Code Style

- Avoid blank lines inside methods and functions
- Avoid using comments whenever possible
- Never declare more than one variable on the same line
- Declare variables as close as possible to where they will be used
- Prefer composition over inheritance whenever possible
