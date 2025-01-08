using System.Diagnostics.CodeAnalysis;
using ErrorOr;

namespace Domain.Boards;

public static class BoardErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Board.NotFound",
        "Board not found.");

    public static readonly Error ColumnAlreadyExists = Error.Validation(
        "Board.ColumnAlreadyExists",
        "A column with the same name already exists.");

    public static readonly Error ColumnNotFound = Error.NotFound(
        "Board.ColumnNotFound",
        "Column not found.");

    public static readonly Error TargetColumnNotFound = Error.NotFound(
        "Board.TargetColumnNotFound",
        "Target column not found.");

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed.")]
    public static Error ColumnHasCards(BoardColumn column) => Error.Validation(
            "Board.ColumnHasCards",
            $"You cannot delete column: {column.Name}. This column has {column.Cards.Count} items in it." +
            "You must first move the items to another column, then try deleting the column again.");

    public static readonly Error CardAlreadyExists = Error.Validation(
        "Board.CardAlreadyExists",
        "A card with the same work item already exists in the column.");

    public static readonly Error InvalidPosition = Error.Validation(
        "Board.InvalidPosition",
        "The position is invalid.");
}