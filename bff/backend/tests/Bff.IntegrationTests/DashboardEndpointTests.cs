using System.Net;
using System.Net.Http.Json;
using Bff.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace Bff.IntegrationTests;

public class DashboardEndpointTests : IClassFixture<BffWebApplicationFactory>
{
    private readonly BffWebApplicationFactory _factory;

    public DashboardEndpointTests(BffWebApplicationFactory factory) => _factory = factory;

    private record DashboardResponse(long businessCount, long businessUnitCount, long userCount, DateTime generatedAt);

    [Fact]
    public async Task Get_dashboard_without_token_returns_401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_dashboard_with_token_aggregates_counts_from_both_domains()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer test-token");

        var response = await client.GetAsync("/api/v1/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<DashboardResponse>();
        body!.businessCount.Should().Be(3);
        body.businessUnitCount.Should().Be(11);
        body.userCount.Should().Be(2);
    }

    [Fact]
    public async Task Get_dashboard_returns_502_when_a_domain_is_down()
    {
        var factory = new BffWebApplicationFactory
        {
            BusinessResponder = _ => throw new HttpRequestException("connection refused"),
        };
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer test-token");

        var response = await client.GetAsync("/api/v1/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.BadGateway);
    }
}
