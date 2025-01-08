using Application.Users.Commands.RegisterUser;
using Application.Users.Commands.UpdateUserProfile;
using Application.Users.Commands.UpdateUserProfilePicture;
using Application.Users.Common;
using Application.Users.Queries.GetCurrentUser;
using Application.Users.Queries.LoginUser;
using Asp.Versioning;
using Contracts.Users;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/users")]
public class UsersController(ISender sender) : ApiController
{
    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <param name="request">The details of the user to register.</param>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterUser(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName);

        var result = await sender.Send(command, cancellationToken);

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
    [ProducesResponseType(typeof(AccessTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginUser(
        LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var query = new LoginUserQuery(request.Email, request.Password);

        var result = await sender.Send(query, cancellationToken);

        if (result.IsError && result.FirstError == UserErrors.InvalidCredentials)
        {
            return Problem(
               statusCode: StatusCodes.Status401Unauthorized,
               title: UserErrors.InvalidCredentials.Description);
        }

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Get the current user.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var query = new GetCurrentUserQuery();

        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            user => Ok(ToDto(user)),
            Problem);
    }

    /// <summary>
    /// Update the profile personal details of the current user.
    /// </summary>
    /// <param name="request">The details of the user to update.</param>
    [HttpPut("profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUserProfile(
        UpdateUserProfileRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserProfileCommand(
            request.FirstName,
            request.LastName);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    /// <summary>
    /// Update the profile picture of the current user.
    /// </summary>
    [HttpPut("profile/picture")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfilePicture(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserProfilePictureCommand(
            file.OpenReadStream(),
            file.ContentType);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    private static UserResponse ToDto(User user)
    {
        return new(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.FullName,
            user.ImageUrl);
    }
}