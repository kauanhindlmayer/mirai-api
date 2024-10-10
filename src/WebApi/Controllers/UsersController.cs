using Application.Users.Commands.RegisterUser;
using Application.Users.Queries.GetCurrentUser;
using Application.Users.Queries.LoginUser;
using Contracts.Users;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/users")]
public class UsersController(ISender _mediator) : ApiController
{
    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <param name="request">The details of the user to register.</param>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterUser(RegisterUserRequest request)
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

    /// <summary>
    /// Log in a user.
    /// </summary>
    /// <param name="request">The details of the user to log in.</param>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginUser(LoginUserRequest request)
    {
        var query = new LoginUserQuery(request.Email, request.Password);

        var result = await _mediator.Send(query);

        return result.Match(
            accessToken => Ok(accessToken.Value),
            Problem);
    }

    /// <summary>
    /// Get the current user.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var query = new GetCurrentUserQuery();

        var result = await _mediator.Send(query);

        return result.Match(
            user => Ok(ToDto(user)),
            Problem);
    }

    private static UserResponse ToDto(User user)
    {
        return new(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName);
    }
}