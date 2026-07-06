using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Bff.Infrastructure.Http;

/// <summary>
/// Relays the caller's bearer token to downstream domain APIs so authorization is
/// enforced end-to-end rather than trusting the BFF as an anonymous super-caller.
/// </summary>
public class BearerTokenPropagationHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BearerTokenPropagationHandler(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization is null)
        {
            var incoming = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
            if (!string.IsNullOrWhiteSpace(incoming)
                && AuthenticationHeaderValue.TryParse(incoming, out var header))
            {
                request.Headers.Authorization = header;
            }
        }

        return base.SendAsync(request, cancellationToken);
    }
}
