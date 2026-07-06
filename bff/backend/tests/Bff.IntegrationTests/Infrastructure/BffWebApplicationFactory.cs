using Bff.Application.Abstractions;
using Bff.Infrastructure.Clients;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Bff.IntegrationTests.Infrastructure;

/// <summary>
/// Boots the real BFF pipeline (controllers, MediatR, aggregation handlers, middleware)
/// while replacing two seams: token validation (Test scheme) and the downstream HTTP
/// transport (stub). Everything in between is exercised for real.
/// </summary>
public class BffWebApplicationFactory : WebApplicationFactory<Program>
{
    /// <summary>Responds to Business Units domain calls. Overridable per test.</summary>
    public Func<HttpRequestMessage, HttpResponseMessage> BusinessResponder { get; set; }
        = DownstreamResponses.DefaultBusiness;

    /// <summary>Responds to OAuth (Users) domain calls. Overridable per test.</summary>
    public Func<HttpRequestMessage, HttpResponseMessage> UserResponder { get; set; }
        = DownstreamResponses.DefaultUsers;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            // 1. Replace token validation with a deterministic test scheme.
            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName, _ => { });

            // 2. Replace the downstream transport of each typed client with a stub.
            services.AddHttpClient<IBusinessCatalogApi, BusinessCatalogApiClient>()
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new StubHttpMessageHandler(req => BusinessResponder(req)));

            services.AddHttpClient<IUserDirectoryApi, UserDirectoryApiClient>()
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new StubHttpMessageHandler(req => UserResponder(req)));
        });
    }
}
