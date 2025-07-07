using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Presentation.Constants;

namespace Presentation.Controllers;

public record AcceptHeaderRequest
{
    [FromHeader(Name = "Accept")]
    public string? Accept { get; init; }

    public bool IncludeLinks =>
        MediaTypeHeaderValue.TryParse(Accept, out MediaTypeHeaderValue? mediaType) &&
        mediaType.SubTypeWithoutSuffix.HasValue &&
        mediaType.SubTypeWithoutSuffix.Value.Contains(CustomMediaTypeNames.Application.HateoasSubType);
}
