namespace Infrastructure.Authentication.Models;

internal sealed class CredentialRepresentationModel
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool Temporary { get; set; }
}