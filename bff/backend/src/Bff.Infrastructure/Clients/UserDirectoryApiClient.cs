using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Bff.Application.Abstractions;
using Bff.Application.Common;
using Bff.Application.Models.Upstream;

namespace Bff.Infrastructure.Clients;

/// <summary>HTTP adapter for the OAuth (Users) domain API.</summary>
public class UserDirectoryApiClient : IUserDirectoryApi
{
    private const string ServiceName = "oauth";
    private const int PageSize = 100;

    // Bounded parallelism for the per-user detail fan-out.
    private const int MaxParallelism = 8;

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;

    public UserDirectoryApiClient(HttpClient http) => _http = http;

    public async Task<long> CountUsersAsync(CancellationToken ct = default)
    {
        var page = await GetAsync<PagedResult<UserSummary>>(
            "api/v1/users?page=1&pageSize=1", ct);
        return page?.TotalCount ?? 0;
    }

    public async Task<IReadOnlyList<UserDetail>> GetUsersWithBusinessPermissionAsync(
        string businessId, int maxUsers, CancellationToken ct = default)
    {
        // 1. Enumerate user summaries (bounded by maxUsers). The summary endpoint does not
        //    carry permissions, so we resolve details per user to know their business scope.
        var ids = await EnumerateUserIdsAsync(maxUsers, ct);

        // 2. Resolve details with bounded parallelism, then keep only those scoped to the business.
        var matched = new ConcurrentBag<UserDetail>();
        using var gate = new SemaphoreSlim(MaxParallelism);

        var tasks = ids.Select(async id =>
        {
            await gate.WaitAsync(ct);
            try
            {
                var detail = await GetAsync<UserDetail>(
                    $"api/v1/users/{Uri.EscapeDataString(id)}", ct);
                if (detail is not null
                    && detail.Permissions.Any(p => p.BusinessId == businessId))
                {
                    matched.Add(detail);
                }
            }
            finally
            {
                gate.Release();
            }
        });

        await Task.WhenAll(tasks);
        return matched.ToList();
    }

    private async Task<IReadOnlyList<string>> EnumerateUserIdsAsync(
        int maxUsers, CancellationToken ct)
    {
        var ids = new List<string>();
        var page = 1;

        while (ids.Count < maxUsers)
        {
            var result = await GetAsync<PagedResult<UserSummary>>(
                $"api/v1/users?page={page}&pageSize={PageSize}", ct);

            if (result is null || result.Items.Count == 0)
                break;

            ids.AddRange(result.Items.Select(u => u.Id));

            if (ids.Count >= result.TotalCount || result.Items.Count < PageSize)
                break;

            page++;
        }

        return ids.Count > maxUsers ? ids.Take(maxUsers).ToList() : ids;
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
