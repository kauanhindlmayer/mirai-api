using Application.Boards.Commands.AddCard;
using Application.Boards.Commands.AddColumn;
using Application.Boards.Commands.CreateBoard;
using Application.Boards.Commands.DeleteBoard;
using Application.Boards.Commands.MoveCard;
using Application.Boards.Commands.RemoveColumn;
using Application.Boards.Queries.GetBoard;
using Application.Boards.Queries.ListBoards;
using Contracts.Boards;
using Domain.Boards;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DomainWorkItemType = Domain.WorkItems.Enums.WorkItemType;

namespace WebApi.Controllers;

[Route("api/projects/{projectId:guid}/boards")]
[AllowAnonymous]
public class BoardsController(ISender _mediator) : ApiController
{
    /// <summary>
    /// Create a new board.
    /// </summary>
    /// <param name="projectId">The ID of the project to create a new board for.</param>
    /// <param name="request">The request to create a new board.</param>
    [HttpPost]
    [ProducesResponseType(typeof(BoardResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBoard(Guid projectId, CreateBoardRequest request)
    {
        var command = new CreateBoardCommand(
            projectId,
            request.Name,
            request.Description);

        var result = await _mediator.Send(command);

        return result.Match(
            board => CreatedAtAction(
                actionName: nameof(GetBoard),
                routeValues: new { ProjectId = projectId, BoardId = board.Id },
                value: ToDto(board)),
            Problem);
    }

    /// <summary>
    /// Get a board by its ID.
    /// </summary>
    /// <param name="boardId">The ID of the board to get.</param>
    [HttpGet("{boardId:guid}")]
    [ProducesResponseType(typeof(BoardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBoard(Guid boardId)
    {
        var query = new GetBoardQuery(boardId);

        var result = await _mediator.Send(query);

        return result.Match(
            board => Ok(ToDto(board)),
            Problem);
    }

    /// <summary>
    /// List all boards.
    /// </summary>
    /// <returns>A list of all boards.</returns>
    /// <param name="projectId">The ID of the project to list boards for.</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<BoardResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListBoards(Guid projectId)
    {
        var query = new ListBoardsQuery(projectId);

        var result = await _mediator.Send(query);

        return result.Match(
            boards => Ok(boards.Select(ToSummaryDto)),
            Problem);
    }

    /// <summary>
    /// Delete a board by its ID. This will also delete all columns and cards on the board.
    /// </summary>
    /// <param name="boardId">The ID of the board to delete.</param>
    [HttpDelete("{boardId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBoard(Guid boardId)
    {
        var command = new DeleteBoardCommand(boardId);

        var result = await _mediator.Send(command);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Add a new column to a board.
    /// </summary>
    /// <param name="projectId">The ID of the project to add a new column to a board.</param>
    /// <param name="boardId">The ID of the board to add a new column to.</param>
    /// <param name="request">The request to add a new column to a board.</param>
    [HttpPost("{boardId:guid}/columns")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddColumn(Guid projectId, Guid boardId, AddColumnRequest request)
    {
        var command = new AddColumnCommand(
            boardId,
            request.Name,
            request.WipLimit,
            request.DefinitionOfDone);

        var result = await _mediator.Send(command);

        return result.Match(
            _ => CreatedAtAction(
                actionName: nameof(GetBoard),
                routeValues: new { ProjectId = projectId, BoardId = boardId },
                value: null),
            Problem);
    }

    /// <summary>
    /// Remove a column from a board. The column must be empty before it can be removed.
    /// </summary>
    /// <param name="boardId">The ID of the board to remove a column from.</param>
    /// <param name="columnId">The ID of the column to remove.</param>
    [HttpDelete("{boardId:guid}/columns/{columnId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveColumn(Guid boardId, Guid columnId)
    {
        var command = new RemoveColumnCommand(boardId, columnId);

        var result = await _mediator.Send(command);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Add a new card to a column on a board.
    /// </summary>
    /// <param name="projectId">The ID of the project to add a new card to a column on a board.</param>
    /// <param name="boardId">The ID of the board to add a new card to a column on.</param>
    /// <param name="columnId">The ID of the column to add a new card to.</param>
    /// <param name="request">The request to add a new card to a column on a board.</param>
    [HttpPost("{boardId:guid}/columns/{columnId:guid}/cards")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddCard(Guid projectId, Guid boardId, Guid columnId, AddCardRequest request)
    {
        if (!DomainWorkItemType.TryFromName(request.Type.ToString(), out var type))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "Invalid work item type");
        }

        var command = new AddCardCommand(boardId, columnId, type, request.Title);

        var result = await _mediator.Send(command);

        return result.Match(
            _ => CreatedAtAction(
                actionName: nameof(GetBoard),
                routeValues: new { ProjectId = projectId, BoardId = boardId },
                value: null),
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
    public async Task<IActionResult> MoveCard(Guid boardId, Guid columnId, Guid cardId, MoveCardRequest request)
    {
        var command = new MoveCardCommand(
            boardId,
            columnId,
            cardId,
            request.TargetColumnId,
            request.TargetPosition);

        var result = await _mediator.Send(command);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    private static BoardResponse ToDto(Board board)
    {
        return new(
            board.Id,
            board.ProjectId,
            board.Name,
            board.Description,
            board.Columns.Select(ToDto));
    }

    private static BoardSummaryResponse ToSummaryDto(Board board)
    {
        return new(board.Id, board.Name);
    }

    private static BoardColumnResponse ToDto(BoardColumn column)
    {
        return new(
            column.Id,
            column.Name,
            column.Position,
            column.WipLimit,
            column.DefinitionOfDone,
            column.Cards.Select(ToDto));
    }

    private static BoardCardResponse ToDto(BoardCard card)
    {
        return new(
            card.Id,
            card.Position,
            card.CreatedAt,
            card.UpdatedAt);
    }
}