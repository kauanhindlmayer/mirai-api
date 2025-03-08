using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Application.Retrospectives.Commands.CreateRetrospectiveItem;
using Domain.Retrospectives;

namespace Application.UnitTests.Retrospectives.Commands;

public class CreateRetrospectiveItemTests
{
    private static readonly CreateRetrospectiveItemCommand Command = new(
        "Test content",
        Guid.NewGuid(),
        Guid.NewGuid());

    private readonly CreateRetrospectiveItemCommandHandler _handler;
    private readonly IRetrospectivesRepository _retrospectivesRepository;
    private readonly IUserContext _userContext;

    public CreateRetrospectiveItemTests()
    {
        _retrospectivesRepository = Substitute.For<IRetrospectivesRepository>();
        _userContext = Substitute.For<IUserContext>();
        _handler = new CreateRetrospectiveItemCommandHandler(
            _retrospectivesRepository,
            _userContext);
    }

    [Fact]
    public async Task Handle_WhenRetrospectiveDoesNotExist_ShouldReturnError()
    {
        _retrospectivesRepository.GetByIdWithColumnsAsync(Command.RetrospectiveId, Arg.Any<CancellationToken>())
            .Returns(null as Retrospective);

        var result = await _handler.Handle(Command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenColumnDoesNotExist_ShouldReturnError()
    {
        var retrospective = new Retrospective(
            "Test Retrospective",
            5,
            Domain.Retrospectives.Enums.RetrospectiveTemplate.StartStopContinue,
            Guid.NewGuid());
        _retrospectivesRepository.GetByIdWithColumnsAsync(Command.RetrospectiveId, Arg.Any<CancellationToken>())
            .Returns(retrospective);

        var result = await _handler.Handle(Command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.ColumnNotFound);
    }

    [Fact]
    public async Task Handle_WhenItemAlreadyExists_ShouldReturnError()
    {
        var retrospective = new Retrospective(
            "Test Retrospective",
            5,
            Domain.Retrospectives.Enums.RetrospectiveTemplate.StartStopContinue,
            Guid.NewGuid());
        var column = new RetrospectiveColumn("Test Column", retrospective.Id);
        var item = new RetrospectiveItem("Test content", column.Id, Guid.NewGuid());
        column.AddItem(item);
        retrospective.AddColumn(column);
        _retrospectivesRepository.GetByIdWithColumnsAsync(Command.RetrospectiveId, Arg.Any<CancellationToken>())
            .Returns(retrospective);

        var result = await _handler.Handle(
            Command with { ColumnId = column.Id },
            CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.ItemAlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldCreateItem()
    {
        var retrospective = new Retrospective(
            "Test Retrospective",
            5,
            Domain.Retrospectives.Enums.RetrospectiveTemplate.StartStopContinue,
            Guid.NewGuid());
        var column = new RetrospectiveColumn("Test Column", retrospective.Id);
        retrospective.AddColumn(column);
        _retrospectivesRepository.GetByIdWithColumnsAsync(Command.RetrospectiveId, Arg.Any<CancellationToken>())
            .Returns(retrospective);

        var result = await _handler.Handle(
            Command with { ColumnId = column.Id },
            CancellationToken.None);

        result.IsError.Should().BeFalse();
        result.Value.Content.Should().Be(Command.Content);
        column.Items.Should().ContainSingle(i => i.Content == Command.Content);
    }
}