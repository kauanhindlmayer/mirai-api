using Application.Retrospectives.Commands.CreateRetrospectiveColumn;
using Domain.Retrospectives;

namespace Application.UnitTests.Retrospectives.Commands;

public class CreateRetrospectiveColumnTests
{
    private static readonly CreateRetrospectiveColumnCommand Command = new(
        "Test Column",
        Guid.NewGuid());

    private readonly CreateRetrospectiveColumnCommandHandler _handler;
    private readonly IRetrospectivesRepository _retrospectivesRepository;

    public CreateRetrospectiveColumnTests()
    {
        _retrospectivesRepository = Substitute.For<IRetrospectivesRepository>();
        _handler = new CreateRetrospectiveColumnCommandHandler(_retrospectivesRepository);
    }

    [Fact]
    public async Task Handle_WhenRetrospectiveDoesNotExist_ShouldReturnError()
    {
        // Arrange
        _retrospectivesRepository.GetByIdWithColumnsAsync(Command.RetrospectiveId, TestContext.Current.CancellationToken)
            .Returns(null as Retrospective);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenColumnAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var retrospective = new Retrospective("Test Retrospective", 5, null, Guid.NewGuid());
        var column = new RetrospectiveColumn("Test Column", retrospective.Id);
        retrospective.AddColumn(column);
        _retrospectivesRepository.GetByIdWithColumnsAsync(Command.RetrospectiveId, TestContext.Current.CancellationToken)
            .Returns(retrospective);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.ColumnAlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenMaxColumnsReached_ShouldReturnError()
    {
        // Arrange
        var retrospective = new Retrospective("Test Retrospective", 5, null, Guid.NewGuid());
        retrospective.AddColumn(new RetrospectiveColumn("Column 4", retrospective.Id));
        retrospective.AddColumn(new RetrospectiveColumn("Column 5", retrospective.Id));
        _retrospectivesRepository.GetByIdWithColumnsAsync(Command.RetrospectiveId, TestContext.Current.CancellationToken)
            .Returns(retrospective);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.MaxColumnsReached);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldCreateColumn()
    {
        // Arrange
        var retrospective = new Retrospective("Test Retrospective", 5, null, Guid.NewGuid());
        _retrospectivesRepository.GetByIdWithColumnsAsync(Command.RetrospectiveId, TestContext.Current.CancellationToken)
            .Returns(retrospective);

        // Act
        var result = await _handler.Handle(Command, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<ErrorOr<Guid>>();
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(retrospective.Id);
        retrospective.Columns.Should().ContainSingle(c => c.Title == Command.Title)
            .Which.Position.Should().Be(retrospective.Columns.Count - 1);
        _retrospectivesRepository.Received().Update(retrospective);
    }
}