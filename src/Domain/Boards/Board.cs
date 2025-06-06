using Domain.Boards.Enums;
using Domain.Common;
using Domain.Teams;
using ErrorOr;

namespace Domain.Boards;

public sealed class Board : AggregateRoot
{
    public Guid TeamId { get; private set; }
    public Team Team { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public List<BoardColumn> Columns { get; private set; } = [];

    public Board(Guid teamId, string name)
    {
        TeamId = teamId;
        Name = name;
        InitializeDefaultColumns(ProcessTemplate.Agile);
    }

    public Board(Team team, string name)
    {
        Team = team;
        TeamId = team.Id;
        Name = name;
        InitializeDefaultColumns(ProcessTemplate.Agile);
    }

    private Board()
    {
    }

    public ErrorOr<BoardColumn> AddColumn(BoardColumn column)
    {
        if (Columns.Any(c => c.Name == column.Name))
        {
            return BoardErrors.ColumnAlreadyExists;
        }

        column.UpdatePosition(Columns.Count);
        Columns.Add(column);
        return column;
    }

    public ErrorOr<BoardColumn> AddColumnAtPosition(BoardColumn column, int position)
    {
        if (position < 0 || position > Columns.Count)
        {
            return BoardErrors.InvalidPosition;
        }

        if (Columns.Any(c => c.Name == column.Name))
        {
            return BoardErrors.ColumnAlreadyExists;
        }

        Columns.Insert(position, column);
        ReorderColumns();
        return column;
    }

    public ErrorOr<Success> RemoveColumn(Guid columnId)
    {
        var column = Columns.SingleOrDefault(c => c.Id == columnId);
        if (column is null)
        {
            return BoardErrors.ColumnNotFound;
        }

        if (column.Cards.Count != 0)
        {
            return BoardErrors.ColumnHasCards(column);
        }

        Columns.Remove(column);
        ReorderColumns();
        return Result.Success;
    }

    public ErrorOr<Success> MoveCard(
        Guid columnId,
        Guid cardId,
        Guid targetColumnId,
        int targetPosition)
    {
        var sourceColumn = Columns.FirstOrDefault(c => c.Id == columnId);
        if (sourceColumn is null)
        {
            return BoardErrors.ColumnNotFound;
        }

        var card = sourceColumn.RemoveCard(cardId);
        if (card.IsError)
        {
            return card.Errors;
        }

        var targetColumn = Columns.FirstOrDefault(c => c.Id == targetColumnId);
        if (targetColumn is null)
        {
            return BoardErrors.ColumnNotFound;
        }

        return targetColumn.AddCardAtPosition(card.Value, targetPosition);
    }

    public ErrorOr<BoardCard> AddCard(BoardCard card)
    {
        var column = Columns.FirstOrDefault(c => c.Id == card.BoardColumnId);
        if (column is null)
        {
            return BoardErrors.ColumnNotFound;
        }

        var result = column.AddCard(card);
        if (result.IsError)
        {
            return result.Errors;
        }

        return result.Value;
    }

    private void InitializeDefaultColumns(ProcessTemplate processTemplate)
    {
        var columnTemplates = BoardColumnTemplates.Templates[processTemplate];
        foreach (var template in columnTemplates)
        {
            AddColumn(new BoardColumn(Id, template.Name, template.WipLimit));
        }
    }

    private void ReorderColumns()
    {
        var position = 0;
        foreach (var column in Columns.OrderBy(c => c.Position))
        {
            column.UpdatePosition(position);
            position++;
        }
    }
}