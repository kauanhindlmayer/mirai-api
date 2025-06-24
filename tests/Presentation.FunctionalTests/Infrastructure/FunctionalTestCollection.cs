namespace Presentation.FunctionalTests.Infrastructure;

[CollectionDefinition(nameof(FunctionalTestCollection))]
public sealed class FunctionalTestCollection : ICollectionFixture<FunctionalTestWebAppFactory>;
