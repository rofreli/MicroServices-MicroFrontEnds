using System.Collections.Immutable;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OAuth.Domain.Interfaces;
using OpenIddict.Abstractions;
using Microsoft.AspNetCore;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OAuth.API.Controllers;

public class AuthorizationController : Controller
{
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly IUserRepository _userRepository;

    public AuthorizationController(IOpenIddictScopeManager scopeManager, IUserRepository userRepository)
        => (_scopeManager, _userRepository) = (scopeManager, userRepository);

    [HttpGet("~/connect/authorize")]
    [HttpPost("~/connect/authorize")]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("OpenIddict request not found.");

        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (result is not { Succeeded: true })
        {
            return Challenge(
                authenticationSchemes: CookieAuthenticationDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties { RedirectUri = $"{Request.Path}{Request.QueryString}" });
        }

        var userId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userRepository.GetByIdAsync(userId!);

        if (user is null || !user.IsActive)
        {
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.AccessDenied,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Account is inactive.",
                }));
        }

        var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        identity.SetClaim(Claims.Subject, user.Id)
                .SetClaim(Claims.Email, user.Email.Value)
                .SetClaim(Claims.Name, user.FullName)
                .SetClaim(Claims.GivenName, user.FirstName)
                .SetClaim(Claims.FamilyName, user.LastName)
                .SetClaim("is_super_admin", user.IsSuperAdmin.ToString().ToLowerInvariant())
                .SetClaim("permissions", JsonSerializer.Serialize(
                    user.Permissions.Select(p => new {
                        businessId = p.BusinessId,
                        businessUnitId = p.BusinessUnitId,
                        module = p.Module,
                        function = p.Function,
                        role = p.Role
                    })));

        identity.SetScopes(request.GetScopes());

        var resources = new List<string>();
        await foreach (var r in _scopeManager.ListResourcesAsync(identity.GetScopes()))
            resources.Add(r);
        identity.SetResources(resources);
        identity.SetDestinations(GetDestinations);

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpPost("~/connect/token")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("OpenIddict request not found.");

        if (!request.IsAuthorizationCodeGrantType() && !request.IsRefreshTokenGrantType())
            throw new InvalidOperationException($"Grant type '{request.GrantType}' is not supported.");

        var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        var userId = result.Principal?.GetClaim(Claims.Subject);
        var user = userId is not null ? await _userRepository.GetByIdAsync(userId) : null;

        if (user is null || !user.IsActive)
        {
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "User not found or inactive.",
                }));
        }

        var identity = new ClaimsIdentity(
            result.Principal!.Claims,
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        identity.SetClaim(Claims.Subject, user.Id)
                .SetClaim(Claims.Email, user.Email.Value)
                .SetClaim(Claims.Name, user.FullName)
                .SetClaim("is_super_admin", user.IsSuperAdmin.ToString().ToLowerInvariant())
                .SetClaim("permissions", JsonSerializer.Serialize(
                    user.Permissions.Select(p => new {
                        businessId = p.BusinessId,
                        businessUnitId = p.BusinessUnitId,
                        module = p.Module,
                        function = p.Function,
                        role = p.Role
                    })));

        identity.SetDestinations(GetDestinations);

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpGet("~/connect/userinfo")]
    public async Task<IActionResult> Userinfo()
    {
        var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        var userId = result.Principal?.GetClaim(Claims.Subject);
        var user = userId is not null ? await _userRepository.GetByIdAsync(userId) : null;

        if (user is null) return Challenge(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        return Ok(new Dictionary<string, object>
        {
            [Claims.Subject] = user.Id,
            [Claims.Email] = user.Email.Value,
            [Claims.Name] = user.FullName,
            [Claims.GivenName] = user.FirstName,
            [Claims.FamilyName] = user.LastName,
            ["isActive"] = user.IsActive,
            ["isSuperAdmin"] = user.IsSuperAdmin,
            ["permissions"] = user.Permissions.Select(p => new {
                businessId = p.BusinessId,
                businessUnitId = p.BusinessUnitId,
                module = p.Module,
                function = p.Function,
                role = p.Role
            }),
        });
    }

    [HttpGet("~/connect/logout")]
    [HttpPost("~/connect/logout")]
    public IActionResult Logout()
    {
        return SignOut(
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        return claim.Type switch
        {
            Claims.Name or Claims.Email or "permissions" or "is_super_admin"
                => new[] { Destinations.AccessToken, Destinations.IdentityToken },
            _ => new[] { Destinations.AccessToken },
        };
    }
}
