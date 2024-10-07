using Application.Users.RegisterUser;
using Contracts.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/users")]
[AllowAnonymous]
public class UsersController(ISender _mediator) : ApiController
{
    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <param name="request">The details of the user to register.</param>
    [HttpPost("register")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName);

        var result = await _mediator.Send(command);

        return result.Match(
            userId => Ok(userId),
            Problem);
    }
}