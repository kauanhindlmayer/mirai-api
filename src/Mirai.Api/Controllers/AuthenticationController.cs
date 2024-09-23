using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.Authentication.Commands.Register;
using Mirai.Application.Authentication.Common;
using Mirai.Application.Authentication.Queries.Login;
using Mirai.Contracts.Authentication;

namespace Mirai.Api.Controllers;

[AllowAnonymous]
public class AuthenticationController(ISender mediator) : ApiController
{
    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <param name="request">The request to register a new user.</param>
    [HttpPost(ApiEndpoints.Authentication.Register)]
    [ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = new RegisterCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password);

        var result = await mediator.Send(command);

        return result.Match(
            authenticationResult => Ok(ToDto(authenticationResult)),
            Problem);
    }

    /// <summary>
    /// Login a user.
    /// </summary>
    /// <param name="request">The request to login a user.</param>
    [HttpPost(ApiEndpoints.Authentication.Login)]
    [ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var query = new LoginQuery(request.Email, request.Password);

        var result = await mediator.Send(query);

        if (result.IsError && result.FirstError == AuthenticationErrors.InvalidCredentials)
        {
            return Problem(
                detail: result.FirstError.Description,
                statusCode: StatusCodes.Status401Unauthorized);
        }

        return result.Match(
            authenticationResult => Ok(ToDto(authenticationResult)),
            Problem);
    }

    public static AuthenticationResponse ToDto(AuthenticationResult authResult)
    {
        return new(
            authResult.User.Id,
            authResult.User.FirstName,
            authResult.User.LastName,
            authResult.User.Email,
            authResult.Token);
    }
}