using Domain.Retrospectives;
using Domain.Retrospectives.Enums;

namespace Domain.UnitTests.Retrospectives;

public class RetrospectiveTests
{
    [Fact]
    public void CreateRetrospective_ShouldSetProperties()
    {
        // Act
        var title = $"Retrospective {DateTime.Now:MMM dd, yyyy}";
        var retrospective = RetrospectiveFactory.CreateRetrospective(title);

        // Assert
        retrospective.TeamId.Should().NotBeEmpty();
        retrospective.Title.Should().Be(title);
        retrospective.MaxVotesPerUser.Should().Be(5);
        retrospective.Columns.Should().HaveCount(3);
    }

    [Fact]
    public void AddColumn_ShouldAddColumn()
    {
        // Arrange
        var retrospective = RetrospectiveFactory.CreateRetrospective();
        var column = RetrospectiveFactory.CreateColumn();

        // Act
        var result = retrospective.AddColumn(column);

        // Assert
        result.IsError.Should().BeFalse();
        retrospective.Columns.Should().Contain(column);
    }

    [Fact]
    public void AddColumn_WhenMaxColumnsReached_ShouldReturnError()
    {
        // Arrange
        var retrospective = RetrospectiveFactory.CreateRetrospective();
        for (var i = 0; i < 5; i++)
        {
            retrospective.AddColumn(RetrospectiveFactory.CreateColumn($"Column {i}"));
        }

        var column = RetrospectiveFactory.CreateColumn("Column 6");

        // Act
        var result = retrospective.AddColumn(column);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.MaxColumnsReached);
    }

    [Fact]
    public void AddColumn_WhenColumnAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var retrospective = RetrospectiveFactory.CreateRetrospective();
        var column = RetrospectiveFactory.CreateColumn();
        retrospective.AddColumn(column);

        // Act
        var result = retrospective.AddColumn(column);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RetrospectiveErrors.ColumnAlreadyExists);
    }

    [Fact]
    public void AddColumn_ShouldUpdateColumnPosition()
    {
        // Arrange
        var retrospective = RetrospectiveFactory.CreateRetrospective();
        retrospective.AddColumn(RetrospectiveFactory.CreateColumn("Column 1"));
        var column = RetrospectiveFactory.CreateColumn("Column 2");

        // Act
        retrospective.AddColumn(column);

        // Assert
        column.Position.Should().Be(1);
    }

    [Fact]
    public void InitializeDefaultColumns_ShouldAddColumns()
    {
        // Arrange
        var columns = RetrospectiveColumnTemplates.Templates[RetrospectiveTemplate.MadSadGlad];

        // Act
        var retrospective = RetrospectiveFactory.CreateRetrospective(template: RetrospectiveTemplate.MadSadGlad);

        // Assert
        retrospective.Columns.Should().HaveCount(3);
        retrospective.Columns.Select(c => c.Title).Should().BeEquivalentTo(columns);
    }

    [Fact]
    public void InitializeDefaultColumns_WhenTemplateIsNull_ShouldUseClassicTemplate()
    {
        // Arrange
        var columns = RetrospectiveColumnTemplates.Templates[RetrospectiveTemplate.Classic];

        // Act
        var retrospective = RetrospectiveFactory.CreateRetrospective(template: null);

        // Assert
        retrospective.Columns.Should().HaveCount(3);
        retrospective.Columns.Select(c => c.Title).Should().BeEquivalentTo(columns);
    }
}