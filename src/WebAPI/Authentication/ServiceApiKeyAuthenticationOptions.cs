#nullable enable
using Microsoft.AspNetCore.Authentication;

namespace WebAPI.Authentication;

/// <summary>
/// Options for the <see cref="ServiceApiKeyAuthenticationHandler"/> scheme.
/// </summary>
public class ServiceApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>The name of this authentication scheme.</summary>
    public const string SchemeName = "ServiceApiKey";

    /// <summary>The header name carrying the shared service API key.</summary>
    public const string HeaderName = "X-Service-Api-Key";

    /// <summary>Gets or sets the expected API key value. When empty, the scheme rejects every request.</summary>
    public string? ApiKey { get; set; }
}
