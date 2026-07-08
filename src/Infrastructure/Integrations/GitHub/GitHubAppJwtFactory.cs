using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Integrations.GitHub;

/// <summary>
/// Builds the short-lived, RS256-signed JWT used to authenticate as the
/// GitHub App itself (as opposed to a specific installation), per GitHub's
/// App authentication scheme.
/// </summary>
internal static class GitHubAppJwtFactory
{
    private static readonly TimeSpan ClockSkewAllowance = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(9);

    public static string CreateJwt(string appId, string privateKeyPem)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem);

        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false },
        };

        var now = DateTime.UtcNow;
        var jwt = new JwtSecurityToken(
            notBefore: now - ClockSkewAllowance,
            expires: now + TokenLifetime,
            signingCredentials: signingCredentials,
            claims:
            [
                new Claim("iat", new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("iss", appId, ClaimValueTypes.String),
            ]);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
