using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bff.IntegrationTests.Infrastructure;

/// <summary>
/// Stands in for OpenIddict token validation during integration tests. A request is
/// treated as authenticated only when it carries an <c>Authorization</c> header, so the
/// 401 path can still be exercised. Claims are shaped from test headers:
/// <c>X-Test-SuperAdmin</c> and <c>X-Test-Permissions</c> (raw JSON).
/// </summary>
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Test";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return Task.FromResult(AuthenticateResult.NoResult());

        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, "test-user") };

        if (Request.Headers.TryGetValue("X-Test-SuperAdmin", out var superAdmin)
            && string.Equals(superAdmin.ToString(), "true", StringComparison.OrdinalIgnoreCase))
        {
            claims.Add(new Claim("is_super_admin", "true"));
        }

        if (Request.Headers.TryGetValue("X-Test-Permissions", out var permissions)
            && !string.IsNullOrWhiteSpace(permissions))
        {
            claims.Add(new Claim("permissions", permissions.ToString()));
        }

        var identity = new ClaimsIdentity(claims, SchemeName);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
