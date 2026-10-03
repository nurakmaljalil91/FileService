#nullable enable
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Domain.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace WebAPI.Authentication;

/// <summary>
/// Authenticates trusted backend services by a shared API key header, so they can read files on behalf of
/// users whose own roles are not allowed to download directly. Successful requests carry the
/// <see cref="FileAccessRoleConstants.Service"/> role.
/// </summary>
public class ServiceApiKeyAuthenticationHandler : AuthenticationHandler<ServiceApiKeyAuthenticationOptions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceApiKeyAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="options">The scheme options monitor.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="encoder">The URL encoder.</param>
    public ServiceApiKeyAuthenticationHandler(
        IOptionsMonitor<ServiceApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    /// <inheritdoc />
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ServiceApiKeyAuthenticationOptions.HeaderName, out var providedKey))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (string.IsNullOrEmpty(Options.ApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Service API key is not configured."));
        }

        var expected = Encoding.UTF8.GetBytes(Options.ApiKey);
        var provided = Encoding.UTF8.GetBytes(providedKey.ToString());
        if (!CryptographicOperations.FixedTimeEquals(expected, provided))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid service API key."));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, Scheme.Name),
            new Claim(ClaimTypes.Role, FileAccessRoleConstants.Service)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));

        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name)));
    }
}
