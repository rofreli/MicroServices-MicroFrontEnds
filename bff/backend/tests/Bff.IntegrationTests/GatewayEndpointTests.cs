using System.Net;
using Bff.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace Bff.IntegrationTests;

/// <summary>
/// Verifies the BFF gateway posture: proxied domain routes require authentication at the
/// gateway (before anything is forwarded to the inner services), while the aggregation
/// endpoints remain owned by MVC and are not swallowed by the proxy catch-all.
/// </summary>
public class GatewayEndpointTests : IClassFixture<BffWebApplicationFactory>
{
    private readonly BffWebApplicationFactory _factory;

    public GatewayEndpointTests(BffWebApplicationFactory factory) => _factory = factory;

    [Theory]
    [InlineData("/api/v1/users")]
    [InlineData("/api/v1/users/some-id")]
    [InlineData("/api/v1/business-units")]
    [InlineData("/api/v1/businesses")]
    public async Task Proxied_domain_route_without_token_is_rejected_at_the_gateway(string path)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(path);

        // The gateway enforces auth before forwarding — the inner service is never reached.
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Aggregation_route_is_owned_by_mvc_not_the_proxy_catch_all()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer test-token");
        client.DefaultRequestHeaders.Add("X-Test-SuperAdmin", "true");

        // /api/v1/businesses/{id}/overview must resolve to the aggregation controller
        // (200 from stubbed clients), not be forwarded by the /api/v1/businesses/** route.
        var response = await client.GetAsync(
            $"/api/v1/businesses/{DownstreamResponses.KnownBusinessId}/overview");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
