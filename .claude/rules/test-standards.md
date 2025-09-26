# Test Standards

## Test Project Structure

### Test Organization

- **Unit Tests** (`tests/[Project].UnitTests/`): Test individual classes and methods in isolation
- **Integration Tests** (`tests/Application.IntegrationTests/`): Test application layer with real database and dependencies
- **Functional Tests** (`tests/Presentation.FunctionalTests/`): End-to-end API testing through HTTP
- **Architecture Tests** (`tests/ArchitectureTests/`): Enforce architectural boundaries and dependencies

### Naming Conventions

- Test files: `[FeatureName]Tests.cs` (e.g., `CreateOrganizationTests.cs`)
- Factory files: `[EntityName]Factory.cs` (e.g., `WorkItemFactory.cs`)
- Base test classes: `Base[TestType]Test.cs` (e.g., `BaseIntegrationTest.cs`)
- Test methods: `[MethodName]_[Scenario]_[ExpectedResult]` (e.g., `CreateOrganization_WhenValidCommand_ShouldCreateOrganization`)

## Test Method Structure

### Follow AAA Pattern

Always structure test methods using Arrange-Act-Assert:

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange
    var input = CreateTestData();

    // Act
    var result = await _service.MethodName(input);

    // Assert
    result.Should().NotBeNull();
}
```

### Test Method Guidelines

- Use descriptive test method names that describe the scenario and expected outcome
- Keep test methods focused on testing one specific behavior
- Avoid complex logic in test methods
- Each test should be independent and not rely on other tests
- Use `async Task` for async tests, never `async void`

## Assertion Standards

### Use FluentAssertions

- Prefer FluentAssertions over classic Assert methods for better readability
- Use specific assertion methods: `.Should().BeTrue()`, `.Should().Be(expected)`, `.Should().BeEquivalentTo()`
- Use `.Should().HaveCount()` for collections
- Use `.Should().Contain()` and `.Should().NotContain()` for collection membership

### Error Testing

- For ErrorOr results: Use `result.IsError.Should().BeTrue()` and `result.FirstError.Should().Be(ExpectedError)`
- Test both success and error scenarios for methods that return ErrorOr types
- Verify specific error types when testing failure cases

## Test Data Management

### Use Factory Pattern

- Create factory classes for test data generation (e.g., `WorkItemFactory`, `OrganizationFactory`)
- Provide default values as constants in factory classes
- Allow parameter overrides for specific test scenarios
- Keep factory methods simple and focused

### Factory Guidelines

```csharp
internal static class EntityFactory
{
    public const string DefaultProperty = "Default Value";
    public static readonly Guid DefaultId = Guid.NewGuid();

    public static Entity Create(
        string? property = null,
        Guid? id = null)
    {
        return new Entity(
            property ?? DefaultProperty,
            id ?? DefaultId);
    }
}
```

## Test Categories

### Unit Tests

- Test domain entities, value objects, and business logic in isolation
- Use mocks (NSubstitute) for external dependencies
- Focus on behavior verification, not implementation details
- Test edge cases and validation rules

### Integration Tests

- Test application layer with real database and infrastructure
- Inherit from `BaseIntegrationTest` for consistent setup
- Use real MediatR `ISender` for command/query execution
- Clean up test data appropriately

### Functional Tests

- Test complete HTTP request/response cycles
- Inherit from `BaseFunctionalTest` for HTTP client setup
- Test authentication and authorization scenarios
- Verify HTTP status codes and response content
- Use `Routes` constants for endpoint URLs

## Test Fixtures and Collections

### Use Test Collections

- Mark test classes with `[Collection(nameof(TestCollection))]` for shared fixtures
- Use `IClassFixture<TFixture>` for test class-specific setup
- Implement proper disposal of resources in fixtures

### Base Test Classes

- Create abstract base classes for common test setup
- Provide shared dependencies through protected fields
- Keep base classes focused and minimal

## Async Testing

### Async Best Practices

- Always use `async Task` for async test methods
- Pass `TestContext.Current.CancellationToken` to async operations when available
- Test timeout scenarios when appropriate
- Avoid `Task.Wait()` or `.Result` in test code

## Test Data Cleanup

### Integration Tests

- Let the test framework handle database cleanup between tests
- Use transactions when possible for faster cleanup
- Avoid dependencies on specific database state

### Functional Tests

- Use unique identifiers to avoid test data conflicts
- Clean up external resources when necessary

## Code Coverage

### Coverage Guidelines

- Aim for high coverage of business logic (domain and application layers)
- Don't pursue 100% coverage at the expense of test quality
- Focus on testing critical paths and edge cases
- Use coverage reports to identify untested scenarios

## Test Documentation

### Documentation Standards

- Test names should be self-documenting
- Add comments only when test logic is complex
- Document test data setup when non-obvious
- Explain complex assertion scenarios when necessary
