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
    /// Get a board by its ID.
    /// </summary>
    /// <param name="boardId">The ID of the board to get.</param>
    [HttpGet("{boardId:guid}")]
    [ProducesResponseType(typeof(BoardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBoard(
        Guid boardId,
        CancellationToken cancellationToken)
    {
        var query = new GetBoardQuery(boardId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// List all boards.
    /// </summary>
    /// <returns>A list of all boards.</returns>
    /// <param name="projectId">The ID of the project to list boards for.</param>
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
    /// Delete a board by its ID. This will also delete all columns and cards on the board.
    /// </summary>
    /// <param name="boardId">The ID of the board to delete.</param>
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
    /// Create a new column on a board.
    /// </summary>
    /// <param name="teamId">The ID of the team to create a new column on a board.</param>
    /// <param name="boardId">The ID of the board to create a new column on.</param>
    /// <param name="request">The request to create a new column on a board.</param>
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
    /// Delete a column by its ID. The column must be empty of cards to be deleted.
    /// </summary>
    /// <param name="boardId">The ID of the board to delete a column from.</param>
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
    /// Create a new card in a column on a board.
    /// </summary>
    /// <param name="teamId">The ID of the team to create a new card on a board.</param>
    /// <param name="boardId">The ID of the board to create a new card on.</param>
    /// <param name="columnId">The ID of the column to create a new card in.</param>
    /// <param name="request">The request to create a new card in a column on a board.</param>
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
    /// Move a card to a new column and position on a board.
    /// </summary>
    /// <param name="boardId">The ID of the board to move a card on.</param>
    /// <param name="columnId">The ID of the column to move a card in.</param>
    /// <param name="cardId">The ID of the card to move.</param>
    /// <param name="request">The request to move a card to a new position in a column.</param>
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