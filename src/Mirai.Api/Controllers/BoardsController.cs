using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.Boards.Commands.CreateBoard;
using Mirai.Contracts.Boards;
using Mirai.Domain.Boards;

namespace Mirai.Api.Controllers;

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
        // var query = new GetBoardQuery(boardId);

        // var result = await _mediator.Send(query);

        // return result.Match(
        //     board => Ok(ToDto(board)),
        //     Problem);
        await Task.CompletedTask;
        return Ok();
    }

    private static BoardResponse ToDto(Board board)
    {
        return new BoardResponse(
            board.Id,
            board.ProjectId,
            board.Name,
            board.Description,
            []);
    }
}