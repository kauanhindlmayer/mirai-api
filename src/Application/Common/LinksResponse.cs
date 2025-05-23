﻿using System.Text.Json.Serialization;

namespace Application.Common;

public abstract class LinksResponse
{
    [JsonPropertyName("_links")]
    public List<LinkResponse> Links { get; set; } = [];
}
