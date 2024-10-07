namespace Infrastructure.Authentication.Models;

public sealed class CredentialRepresentationModel
{
    public string Value { get; set; } = string.Empty;
    public bool Temporary { get; set; }
    public string Type { get; set; } = string.Empty;
}