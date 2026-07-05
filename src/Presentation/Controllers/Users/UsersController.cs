using System.Net.Mime;
using Application.Abstractions;
using Application.Users.Commands.ForgotPassword;
using Application.Users.Commands.LoginWithGitHub;
using Application.Users.Commands.RegisterUser;
using Application.Users.Commands.ResetPassword;
using Application.Users.Commands.UpdateUserAvatar;
using Application.Users.Commands.UpdateUserProfile;
using Application.Users.Queries.GetCurrentUser;
using Application.Users.Queries.GetUserAvatar;
using Application.Users.Queries.LoginUser;
using Asp.Versioning;
using Domain.Users;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Constants;

namespace Presentation.Controllers.Users;

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
    /// Log in a user with GitHub.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login/github")]
    [ProducesResponseType(typeof(AccessTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AccessTokenResponse>> LoginWithGitHub(
        LoginWithGitHubRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginWithGitHubCommand(request.Code, request.RedirectUri);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsError && result.FirstError == UserErrors.InvalidCredentials)
        {
            return Problem(
               statusCode: StatusCodes.Status401Unauthorized,
               title: UserErrors.InvalidCredentials.Description);
        }

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Request a password reset email.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ForgotPassword(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ForgotPasswordCommand(request.Email);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    /// <summary>
    /// Reset a user's password using a reset token.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResetPassword(
        ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ResetPasswordCommand(
            request.Email,
            request.Token,
            request.NewPassword);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(),
            Problem);
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
    /// Update the avatar of the current user.
    /// </summary>
    [HttpPatch("avatar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateAvatar(
        [FromForm] UpdateAvatarRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserAvatarCommand(request.File);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    /// <summary>
    /// Get a user's avatar image.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    [AllowAnonymous]
    [HttpGet("{userId:guid}/avatar")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetUserAvatar(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var query = new GetUserAvatarQuery(userId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            file => File(file.Stream, file.ContentType),
            Problem);
    }

    private List<LinkResponse> CreateLinksForUser()
    {
        List<LinkResponse> links =
        [
            _linkService.Create(nameof(GetCurrentUser), "self", HttpMethods.Get),
            _linkService.Create(nameof(UpdateUserProfile), "update-profile", HttpMethods.Put),
            _linkService.Create(nameof(UpdateAvatar), "update-avatar", HttpMethods.Patch),
        ];

        return links;
    }
}