using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OAuth.Domain.Interfaces;
using DomainUser = OAuth.Domain.Entities.User;

namespace OAuth.API.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class AccountController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _hasher;

    public AccountController(IUserRepository userRepository, IPasswordHasher hasher)
        => (_userRepository, _hasher) = (userRepository, hasher);

    [HttpGet("/account/login")]
    public IActionResult Login([FromQuery] string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost("/account/login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        string email, string password, string? returnUrl = null)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null || user.PasswordHash is null || !_hasher.Verify(password, user.PasswordHash))
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Error"] = "Email ou senha inválidos.";
            return View();
        }

        if (!user.IsActive)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Error"] = "Conta inativa. Entre em contato com o administrador.";
            return View();
        }

        await SignInWithCookieAsync(user);
        return LocalRedirect(returnUrl ?? "/");
    }

    [HttpPost("/account/logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }

    [HttpGet("/account/challenge/{provider}")]
    public IActionResult ExternalChallenge(string provider, [FromQuery] string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalCallback), "Account", new { returnUrl });
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, provider);
    }

    [HttpGet("/account/callback")]
    public async Task<IActionResult> ExternalCallback([FromQuery] string? returnUrl = null)
    {
        var result = await HttpContext.AuthenticateAsync("ExternalCookie");
        if (!result.Succeeded) return RedirectToAction(nameof(Login));

        var email = result.Principal?.FindFirstValue(ClaimTypes.Email);
        var providerKey = result.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        var provider = result.Properties?.Items[".AuthScheme"] ?? "Unknown";
        var firstName = result.Principal?.FindFirstValue(ClaimTypes.GivenName) ?? "User";
        var lastName = result.Principal?.FindFirstValue(ClaimTypes.Surname) ?? string.Empty;

        await HttpContext.SignOutAsync("ExternalCookie");

        if (email is null || providerKey is null)
            return RedirectToAction(nameof(Login));

        var user = await _userRepository.GetByExternalProviderAsync(provider, providerKey)
                ?? await _userRepository.GetByEmailAsync(email);

        if (user is null)
        {
            user = DomainUser.CreateFromExternalProvider(email, firstName, lastName, provider, providerKey);
            await _userRepository.AddAsync(user);
        }
        else
        {
            user.LinkExternalProvider(provider, providerKey);
            await _userRepository.UpdateAsync(user);
        }

        if (!user.IsActive)
        {
            ViewData["Error"] = "Conta inativa. Entre em contato com o administrador.";
            return View("Login");
        }

        await SignInWithCookieAsync(user);
        return LocalRedirect(returnUrl ?? "/");
    }

    private async Task SignInWithCookieAsync(DomainUser user)
    {
        var permissionsJson = JsonSerializer.Serialize(
            user.Permissions.Select(p => new {
                businessId = p.BusinessId,
                businessUnitId = p.BusinessUnitId,
                module = p.Module,
                function = p.Function,
                role = p.Role
            }));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email.Value),
            new(ClaimTypes.Name, user.FullName),
            new("is_super_admin", user.IsSuperAdmin.ToString().ToLowerInvariant()),
            new("permissions", permissionsJson),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) });
    }
}
