using System.Net.Mime;
using Application.Common;
using Application.Users.Commands.RegisterUser;
using Application.Users.Commands.UpdateUserProfile;
using Application.Users.Commands.UpdateUserProfilePicture;
using Application.Users.Common;
using Application.Users.Queries.GetCurrentUser;
using Application.Users.Queries.LoginUser;
using Asp.Versioning;
using Contracts.Users;
using Domain.Users;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Constants;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/users")]
[Produces(
    MediaTypeNames.Application.Json,
    CustomMediaTypeNames.Application.JsonV1,
    CustomMediaTypeNames.Application.HateoasJson,
    CustomMediaTypeNames.Application.HateoasJsonV1)]
public sealed class UsersController : ApiController
{
    private readonly ISender _sender;
    private readonly LinkService _linkService;

    public UsersController(
        ISender sender,
        LinkService linkService)
    {
        _sender = sender;
        _linkService = linkService;
    }

    /// <summary>
    /// Register a user.
    /// </summary>
    /// <returns>The unique identifier of the registered user.</returns>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> RegisterUser(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            userId => Ok(userId),
            Problem);
    }

    /// <summary>
    /// Log in a user.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AccessTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AccessTokenResponse>> LoginUser(
        LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var query = new LoginUserQuery(request.Email, request.Password);

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsError && result.FirstError == UserErrors.InvalidCredentials)
        {
            return Problem(
               statusCode: StatusCodes.Status401Unauthorized,
               title: UserErrors.InvalidCredentials.Description);
        }

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Get the current user details.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserResponse>> GetCurrentUser(
        [FromHeader] AcceptHeaderRequest acceptHeader,
        CancellationToken cancellationToken)
    {
        var query = new GetCurrentUserQuery();

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            user =>
            {
                if (acceptHeader.IncludeLinks)
                {
                    user.Links = CreateLinksForUser();
                }

                return Ok(user);
            },
            Problem);
    }

    /// <summary>
    /// Update the profile of the current user.
    /// </summary>
    [HttpPut("profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateUserProfile(
        UpdateUserProfileRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserProfileCommand(
            request.FirstName,
            request.LastName);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    /// <summary>
    /// Update the profile picture of the current user.
    /// </summary>
    [HttpPatch("profile/picture")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateProfilePicture(
        [FromForm] UpdateProfilePictureRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserProfilePictureCommand(request.File);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    private List<LinkResponse> CreateLinksForUser()
    {
        List<LinkResponse> links =
        [
            _linkService.Create(nameof(GetCurrentUser), "self", HttpMethods.Get),
            _linkService.Create(nameof(UpdateUserProfile), "update-profile", HttpMethods.Put),
            _linkService.Create(nameof(UpdateProfilePicture), "update-profile-picture", HttpMethods.Patch),
        ];

        return links;
    }
}