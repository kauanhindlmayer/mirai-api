using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.Boards.Commands.AddCard;
using Mirai.Application.Boards.Commands.AddColumn;
using Mirai.Application.Boards.Commands.CreateBoard;
using Mirai.Application.Boards.Commands.RemoveColumn;
using Mirai.Application.Boards.Queries.GetBoard;
using Mirai.Contracts.Boards;
using Mirai.Domain.Boards;
using DomainWorkItemType = Mirai.Domain.WorkItems.Enums.WorkItemType;

namespace Mirai.Api.Controllers;

[AllowAnonymous]
public class BoardsController(ISender _mediator) : ApiController
{
    /// <summary>
    /// Create a new board.
    /// </summary>
    /// <param name="request">The request to create a new board.</param>
    [HttpPost(ApiEndpoints.Boards.Create)]
    [ProducesResponseType(typeof(BoardResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBoard(CreateBoardRequest request)
    {
        var command = new CreateBoardCommand(
            request.ProjectId,
            request.Name,
            request.Description);

        var result = await _mediator.Send(command);

        return result.Match(
            board => CreatedAtAction(
                actionName: nameof(GetBoard),
                routeValues: new { BoardId = board.Id },
                value: ToDto(board)),
            Problem);
    }

    /// <summary>
    /// Get a board by its ID.
    /// </summary>
    /// <param name="boardId">The ID of the board to get.</param>
    [HttpGet(ApiEndpoints.Boards.Get)]
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
    /// Add a new column to a board.
    /// </summary>
    /// <param name="boardId">The ID of the board to add a new column to.</param>
    /// <param name="request">The request to add a new column to a board.</param>
    [HttpPost(ApiEndpoints.Boards.AddColumn)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddColumn(Guid boardId, AddColumnRequest request)
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
                routeValues: new { BoardId = boardId },
                value: null),
            Problem);
    }

    /// <summary>
    /// Remove a column from a board.
    /// </summary>
    /// <param name="boardId">The ID of the board to remove a column from.</param>
    /// <param name="columnId">The ID of the column to remove.</param>
    [HttpDelete(ApiEndpoints.Boards.RemoveColumn)]
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
    /// <param name="boardId">The ID of the board to add a new card to a column on.</param>
    /// <param name="columnId">The ID of the column to add a new card to.</param>
    /// <param name="request">The request to add a new card to a column on a board.</param>
    [HttpPost(ApiEndpoints.Boards.AddCard)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddCard(Guid boardId, Guid columnId, AddCardRequest request)
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
                routeValues: new { BoardId = boardId },
                value: null),
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