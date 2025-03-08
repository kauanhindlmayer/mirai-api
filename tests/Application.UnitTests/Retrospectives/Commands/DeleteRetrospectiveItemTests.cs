using Application.Common.Interfaces.Persistence;
using Application.Retrospectives.Commands.DeleteRetrospectiveItem;
using Domain.Retrospectives;

namespace Application.UnitTests.Retrospectives.Commands;

public class DeleteRetrospectiveItemTests
{
    private static readonly DeleteRetrospectiveItemCommand Command = new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        Guid.NewGuid());

    private readonly DeleteRetrospectiveItemCommandHandler _handler;
    private readonly IRetrospectivesRepository _retrospectivesRepository;

    public DeleteRetrospectiveItemTests()
    {
        _retrospectivesRepository = Substitute.For<IRetrospectivesRepository>();
        _handler = new DeleteRetrospectiveItemCommandHandler(_retrospectivesRepository);
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
            null,
            Guid.NewGuid());
        _retrospectivesRepository.GetByIdWithColumnsAsync(Command.RetrospectiveId, Arg.Any<CancellationToken>())
            .Returns(retrospective);

        var result = await _handler.Handle(Command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.ColumnNotFound);
    }

    [Fact]
    public async Task Handle_WhenItemDoesNotExist_ShouldReturnError()
    {
        var retrospective = new Retrospective(
            "Test Retrospective",
            5,
            null,
            Guid.NewGuid());
        var column = new RetrospectiveColumn("Test Column", Command.RetrospectiveId);
        retrospective.AddColumn(column);
        _retrospectivesRepository.GetByIdWithColumnsAsync(Command.RetrospectiveId, Arg.Any<CancellationToken>())
            .Returns(retrospective);

        var result = await _handler.Handle(
            Command with { ColumnId = column.Id },
            CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.ItemNotFound);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldDeleteItem()
    {
        var retrospective = new Retrospective(
            "Test Retrospective",
            5,
            null,
            Guid.NewGuid());
        var column = new RetrospectiveColumn("Test Column", Command.RetrospectiveId);
        var item = new RetrospectiveItem("Test content", column.Id, Command.ItemId);
        column.AddItem(item);
        retrospective.AddColumn(column);
        _retrospectivesRepository.GetByIdWithColumnsAsync(Command.RetrospectiveId, Arg.Any<CancellationToken>())
            .Returns(retrospective);

        var result = await _handler.Handle(
            Command with { ColumnId = column.Id, ItemId = item.Id },
            CancellationToken.None);

        result.IsError.Should().BeFalse();
        column.Items.Should().NotContain(i => i.Id == Command.ItemId);
        _retrospectivesRepository.Received().Update(retrospective);
    }
}