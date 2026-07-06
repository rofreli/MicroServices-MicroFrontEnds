using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Bff.Application.Abstractions;
using Bff.Application.Common;
using Bff.Application.Models.Upstream;

namespace Bff.Infrastructure.Clients;

/// <summary>HTTP adapter for the Business Units domain API.</summary>
public class BusinessCatalogApiClient : IBusinessCatalogApi
{
    private const string ServiceName = "business-units";

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;

    public BusinessCatalogApiClient(HttpClient http) => _http = http;

    public async Task<long> CountBusinessesAsync(CancellationToken ct = default)
    {
        // pageSize=1 keeps the payload tiny; we only read TotalCount from the envelope.
        var page = await GetAsync<PagedResult<BusinessDetail>>(
            "api/v1/businesses?page=1&pageSize=1", ct);
        return page?.TotalCount ?? 0;
    }

    public async Task<long> CountBusinessUnitsAsync(CancellationToken ct = default)
    {
        var page = await GetAsync<PagedResult<BusinessUnitSummary>>(
            "api/v1/business-units?page=1&pageSize=1", ct);
        return page?.TotalCount ?? 0;
    }

    public Task<BusinessDetail?> GetBusinessAsync(string businessId, CancellationToken ct = default)
        => GetAsync<BusinessDetail>(
            $"api/v1/businesses/{Uri.EscapeDataString(businessId)}", ct);

    public async Task<PagedResult<BusinessUnitSummary>> GetBusinessUnitsAsync(
        string businessId, int page, int pageSize, CancellationToken ct = default)
    {
        var result = await GetAsync<PagedResult<BusinessUnitSummary>>(
            $"api/v1/businesses/{Uri.EscapeDataString(businessId)}/business-units?page={page}&pageSize={pageSize}",
            ct);
        return result ?? PagedResult<BusinessUnitSummary>.Empty;
    }

    private async Task<T?> GetAsync<T>(string relativeUrl, CancellationToken ct)
    {
        HttpResponseMessage response;
        try
        {
            response = await _http.GetAsync(relativeUrl, ct);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            throw new UpstreamException(ServiceName, ex.Message, ex);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
            return default;

        if (!response.IsSuccessStatusCode)
            throw new UpstreamException(
                ServiceName, $"returned status {(int)response.StatusCode}.");

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, ct);
    }
}
