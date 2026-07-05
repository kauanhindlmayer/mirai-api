namespace Infrastructure.Authentication.Models;

internal sealed class IdentityProviderRepresentationModel
{
    public required string Alias { get; init; }
    public required string ProviderId { get; init; }
    public bool Enabled { get; init; } = true;
    public bool TrustEmail { get; init; } = true;
    public required Dictionary<string, string> Config { get; init; }

    internal static IdentityProviderRepresentationModel FromClientCredentials(
        string clientId,
        string clientSecret) =>
        new()
        {
            Alias = "github",
            ProviderId = "github",
            Config = new Dictionary<string, string>
            {
                ["clientId"] = clientId,
                ["clientSecret"] = clientSecret,
                ["defaultScope"] = "user:email",
                ["useJwksUrl"] = "true",
            },
        };
}
