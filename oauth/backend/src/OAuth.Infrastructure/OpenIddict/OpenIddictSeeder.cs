using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OAuth.Infrastructure.OpenIddict;

public class OpenIddictSeeder : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly OpenIddictSeederOptions _options;

    public OpenIddictSeeder(IServiceProvider serviceProvider, IOptions<OpenIddictSeederOptions> options)
        => (_serviceProvider, _options) = (serviceProvider, options.Value);

    public async Task StartAsync(CancellationToken ct)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        await SeedScopesAsync(scopeManager, ct);
        await SeedSpaClientAsync(manager, ct);
    }

    private static async Task SeedScopesAsync(IOpenIddictScopeManager manager, CancellationToken ct)
    {
        if (await manager.FindByNameAsync("api", ct) is null)
        {
            await manager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "api",
                DisplayName = "Platform API",
                Resources = { "resource_server" }
            }, ct);
        }

        if (await manager.FindByNameAsync("roles", ct) is null)
        {
            await manager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "roles",
                DisplayName = "User Roles",
            }, ct);
        }
    }

    private async Task SeedSpaClientAsync(IOpenIddictApplicationManager manager, CancellationToken ct)
    {
        if (await manager.FindByClientIdAsync("spa", ct) is not null) return;

        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = "spa",
            DisplayName = "Platform SPA",
            ClientType = ClientTypes.Public,
            ApplicationType = ApplicationTypes.Web,
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Logout,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.ResponseTypes.Code,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Prefixes.Scope + "roles",
                Permissions.Prefixes.Scope + "api",
            }
        };

        descriptor.RedirectUris.UnionWith(
            _options.SpaRedirectUris.Select(u => new Uri(u)));
        descriptor.PostLogoutRedirectUris.UnionWith(
            _options.SpaPostLogoutUris.Select(u => new Uri(u)));

        await manager.CreateAsync(descriptor, ct);
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}

public class OpenIddictSeederOptions
{
    public List<string> SpaRedirectUris { get; set; } = new() { "http://localhost:3000/auth/callback" };
    public List<string> SpaPostLogoutUris { get; set; } = new() { "http://localhost:3000" };
}
