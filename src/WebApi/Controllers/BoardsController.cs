using Application.Boards.Commands.CreateCard;
using Application.Boards.Commands.CreateColumn;
using Application.Boards.Commands.DeleteBoard;
using Application.Boards.Commands.DeleteColumn;
using Application.Boards.Commands.MoveCard;
using Application.Boards.Queries.GetBoard;
using Application.Boards.Queries.ListBoards;
using Asp.Versioning;
using Contracts.Boards;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/teams/{teamId:guid}/boards")]
public class BoardsController : ApiController
{
    private readonly ISender _sender;

    public BoardsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Retrieve a board.
    /// </summary>
    /// <remarks>
    /// Retrieves the board with the specified unique identifier.
    /// </remarks>
    /// <param name="teamId">The team's unique identifier.</param>
    /// <param name="boardId">The board's unique identifier.</param>
    [HttpGet("{boardId:guid}")]
    [ProducesResponseType(typeof(BoardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBoard(
        Guid teamId,
        Guid boardId,
        CancellationToken cancellationToken)
    {
        var query = new GetBoardQuery(teamId, boardId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Retrieve all boards for a project.
    /// </summary>
    /// <remarks>
    /// Returns a list of boards for the specified project.
    /// </remarks>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpGet("/api/v{version:apiVersion}/projects/{projectId:guid}/boards")]
    [ProducesResponseType(typeof(IReadOnlyList<BoardResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListBoards(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var query = new ListBoardsQuery(projectId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Delete a board.
    /// </summary>
    /// <remarks>
    /// Deletes the board with the specified unique identifier. Deleting is only
    /// possible if the board does not have any columns associated with it.
    /// </remarks>
    /// <param name="boardId">The board's unique identifier.</param>
    [HttpDelete("{boardId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBoard(
        Guid boardId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteBoardCommand(boardId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Create a column in the board.
    /// </summary>
    /// <remarks>
    /// Creates a new column in the board.
    /// </remarks>
    /// <param name="teamId">The team's unique identifier.</param>
    /// <param name="boardId">The board's unique identifier.</param>
    [HttpPost("{boardId:guid}/columns")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateColumn(
        Guid teamId,
        Guid boardId,
        CreateColumnRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateColumnCommand(
            boardId,
            request.Name,
            request.WipLimit,
            request.DefinitionOfDone,
            request.Position);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            columnId => CreatedAtAction(
                nameof(GetBoard),
                new { teamId, boardId },
                columnId),
            Problem);
    }

    /// <summary>
    /// Delete a column from the board.
    /// </summary>
    /// <remarks>
    /// Deletes the column with the specified unique identifier. Deleting is
    /// only possible if the column does not have any cards associated with it.
    /// </remarks>
    /// <param name="boardId">The board's unique identifier.</param>
    /// <param name="columnId">The ID of the column to delete.</param>
    [HttpDelete("{boardId:guid}/columns/{columnId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteColumn(
        Guid boardId,
        Guid columnId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteColumnCommand(boardId, columnId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Create a card in a column.
    /// </summary>
    /// <remarks>
    /// Creates a new card in the specified column.
    /// </remarks>
    /// <param name="teamId">The team's unique identifier.</param>
    /// <param name="boardId">The board's unique identifier.</param>
    /// <param name="columnId">The column's unique identifier.</param>
    [HttpPost("{boardId:guid}/columns/{columnId:guid}/cards")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateCard(
        Guid teamId,
        Guid boardId,
        Guid columnId,
        CreateCardRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCardCommand(
            boardId,
            columnId,
            request.Type,
            request.Title);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            cardId => CreatedAtAction(
                nameof(GetBoard),
                new { teamId, boardId },
                cardId),
            Problem);
    }

    /// <summary>
    /// Move a card to a new column and position.
    /// </summary>
    /// <remarks>
    /// Moves a card to a new column and position in the board.
    /// </remarks>
    /// <param name="boardId">The board's unique identifier.</param>
    /// <param name="columnId">The column's unique identifier.</param>
    /// <param name="cardId">The card's unique identifier.</param>
    [HttpPost("{boardId:guid}/columns/{columnId:guid}/cards/{cardId:guid}/move")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MoveCard(
        Guid boardId,
        Guid columnId,
        Guid cardId,
        MoveCardRequest request,
        CancellationToken cancellationToken)
    {
        var command = new MoveCardCommand(
            boardId,
            columnId,
            cardId,
            request.TargetColumnId,
            request.TargetPosition);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}