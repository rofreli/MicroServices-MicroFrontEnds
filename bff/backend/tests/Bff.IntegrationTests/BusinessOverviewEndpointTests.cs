using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Bff.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace Bff.IntegrationTests;

public class BusinessOverviewEndpointTests : IClassFixture<BffWebApplicationFactory>
{
    private readonly BffWebApplicationFactory _factory;

    public BusinessOverviewEndpointTests(BffWebApplicationFactory factory) => _factory = factory;

    private static string PermissionsFor(string businessId) =>
        JsonSerializer.Serialize(new[]
        {
            new { businessId, businessUnitId = (string?)null, module = "Business", function = (string?)null, role = "BusinessAdmin" },
        });

    [Fact]
    public async Task Overview_without_token_returns_401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/v1/businesses/{DownstreamResponses.KnownBusinessId}/overview");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Overview_as_super_admin_composes_units_and_team()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer test-token");
        client.DefaultRequestHeaders.Add("X-Test-SuperAdmin", "true");

        var response = await client.GetAsync($"/api/v1/businesses/{DownstreamResponses.KnownBusinessId}/overview");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = doc.RootElement;

        root.GetProperty("business").GetProperty("id").GetString().Should().Be("biz-1");
        root.GetProperty("businessUnitCount").GetInt64().Should().Be(2);
        root.GetProperty("businessUnits").GetArrayLength().Should().Be(2);

        // Only u-1 has a permission on biz-1; u-2 is scoped to another business.
        var team = root.GetProperty("team");
        team.GetArrayLength().Should().Be(1);
        team[0].GetProperty("userId").GetString().Should().Be("u-1");
        var roles = team[0].GetProperty("roles").EnumerateArray().Select(r => r.GetString()).ToArray();
        roles.Should().BeEquivalentTo(new[] { "Reader", "Writer" });
    }

    [Fact]
    public async Task Overview_with_scoped_permission_is_allowed()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer test-token");
        client.DefaultRequestHeaders.Add("X-Test-Permissions", PermissionsFor("biz-1"));

        var response = await client.GetAsync($"/api/v1/businesses/{DownstreamResponses.KnownBusinessId}/overview");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Overview_without_access_to_business_returns_403()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer test-token");
        client.DefaultRequestHeaders.Add("X-Test-Permissions", PermissionsFor("some-other-business"));

        var response = await client.GetAsync($"/api/v1/businesses/{DownstreamResponses.KnownBusinessId}/overview");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Overview_of_unknown_business_returns_404()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer test-token");
        client.DefaultRequestHeaders.Add("X-Test-SuperAdmin", "true");

        var response = await client.GetAsync("/api/v1/businesses/does-not-exist/overview");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
