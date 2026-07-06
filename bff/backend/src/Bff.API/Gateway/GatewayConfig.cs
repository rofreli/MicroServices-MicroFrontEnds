using System.Net;
using Bff.Infrastructure.Options;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;

namespace Bff.API.Gateway;

/// <summary>
/// Builds the reverse-proxy routing table that turns the BFF into the system's single
/// entry point. Domain data endpoints (<c>/api/v1/**</c>) are forwarded only after the
/// gateway has authenticated the caller; the OIDC/login surface (<c>/connect</c>,
/// <c>/account</c>) is forwarded anonymously (it manages its own auth). The inner services
/// are reached over the private network addresses in <see cref="DownstreamOptions"/> and
/// are never published to the host.
/// </summary>
public static class GatewayConfig
{
    public const string ApiAccessPolicy = "ApiAccess";
    public const string CorsPolicy = "AllowFrontend";

    private const string BusinessUnitsCluster = "businessunits";
    private const string OAuthCluster = "oauth";

    public static (IReadOnlyList<RouteConfig> Routes, IReadOnlyList<ClusterConfig> Clusters)
        Build(DownstreamOptions options)
    {
        var routes = new[]
        {
            // ── Authenticated domain data ─────────────────────────────────────────
            ApiRoute("api-users", "/api/v1/users/{**catch-all}", OAuthCluster),
            ApiRoute("api-business-units", "/api/v1/business-units/{**catch-all}", BusinessUnitsCluster),
            // NOTE: the aggregation controller owns /api/v1/businesses/{id}/overview and
            // /api/v1/dashboard — those attribute routes are more specific than this
            // catch-all, so MVC wins for them and the rest is proxied.
            ApiRoute("api-businesses", "/api/v1/businesses/{**catch-all}", BusinessUnitsCluster),

            // ── OIDC / login surface (anonymous passthrough) ──────────────────────
            AnonymousRoute("oidc-connect", "/connect/{**catch-all}", OAuthCluster),
            AnonymousRoute("oidc-account", "/account/{**catch-all}", OAuthCluster),
        };

        var clusters = new[]
        {
            Cluster(BusinessUnitsCluster, options.BusinessUnitsApi),
            Cluster(OAuthCluster, options.OAuthApi),
        };

        return (routes, clusters);
    }

    private static RouteConfig ApiRoute(string id, string path, string cluster) => new()
    {
        RouteId = id,
        ClusterId = cluster,
        AuthorizationPolicy = ApiAccessPolicy,
        CorsPolicy = CorsPolicy,
        Match = new RouteMatch { Path = path },
    };

    private static RouteConfig AnonymousRoute(string id, string path, string cluster) => new()
    {
        RouteId = id,
        ClusterId = cluster,
        AuthorizationPolicy = "anonymous", // YARP built-in: skip authorization for this route
        CorsPolicy = CorsPolicy,
        Match = new RouteMatch { Path = path },
    };

    private static ClusterConfig Cluster(string id, string address) => new()
    {
        ClusterId = id,
        // Force HTTP/1.1 to the inner services. YARP defaults to HTTP/2 (RequestVersionOrLower);
        // over cleartext h2c to Kestrel that silently drops POST bodies
        // ("Sent 0 request content bytes, but Content-Length promised N"), breaking every
        // form POST (login, OIDC token exchange). Exact 1.1 avoids the broken negotiation.
        HttpRequest = new ForwarderRequestConfig
        {
            Version = HttpVersion.Version11,
            VersionPolicy = HttpVersionPolicy.RequestVersionExact,
        },
        Destinations = new Dictionary<string, DestinationConfig>
        {
            [$"{id}-1"] = new DestinationConfig { Address = address },
        },
    };
}
