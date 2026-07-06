using System.Security.Claims;
using System.Text.Json;
using Bff.Application.Abstractions;

namespace Bff.API.Security;

/// <summary>
/// Reads the caller's identity and authorization scope from the validated access token.
/// The OAuth server emits <c>is_super_admin</c> (string) and <c>permissions</c>
/// (JSON array) claims; both are parsed defensively.
/// </summary>
public class CurrentUser : ICurrentUser
{
    public const string SuperAdminClaim = "is_super_admin";
    public const string PermissionsClaim = "permissions";

    private static readonly JsonSerializerOptions JsonOptions =
        new() { PropertyNameCaseInsensitive = true };

    private readonly Lazy<IReadOnlyList<CallerPermission>> _permissions;
    private readonly ClaimsPrincipal? _principal;

    public CurrentUser(IHttpContextAccessor accessor)
    {
        _principal = accessor.HttpContext?.User;
        _permissions = new Lazy<IReadOnlyList<CallerPermission>>(ParsePermissions);
    }

    public bool IsAuthenticated => _principal?.Identity?.IsAuthenticated ?? false;

    public string? Subject =>
        _principal?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? _principal?.FindFirstValue("sub");

    public bool IsSuperAdmin =>
        string.Equals(_principal?.FindFirstValue(SuperAdminClaim), "true",
            StringComparison.OrdinalIgnoreCase);

    public IReadOnlyList<CallerPermission> Permissions => _permissions.Value;

    public bool CanAccessBusiness(string businessId)
        => IsSuperAdmin
        || Permissions.Any(p => p.BusinessId == businessId);

    private IReadOnlyList<CallerPermission> ParsePermissions()
    {
        var raw = _principal?.FindFirstValue(PermissionsClaim);
        if (string.IsNullOrWhiteSpace(raw))
            return Array.Empty<CallerPermission>();

        try
        {
            return JsonSerializer.Deserialize<List<CallerPermission>>(raw, JsonOptions)
                   ?? (IReadOnlyList<CallerPermission>)Array.Empty<CallerPermission>();
        }
        catch (JsonException)
        {
            // A malformed claim must never crash a request — treat as "no permissions".
            return Array.Empty<CallerPermission>();
        }
    }
}
