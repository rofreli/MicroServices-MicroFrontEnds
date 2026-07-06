using Bff.Application.Abstractions;
using Bff.Infrastructure.Clients;
using Bff.Infrastructure.Http;
using Bff.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bff.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DownstreamOptions>(
            configuration.GetSection(DownstreamOptions.SectionName));

        services.AddHttpContextAccessor();
        services.AddTransient<BearerTokenPropagationHandler>();

        var options = configuration.GetSection(DownstreamOptions.SectionName)
            .Get<DownstreamOptions>() ?? new DownstreamOptions();
        var timeout = TimeSpan.FromSeconds(Math.Clamp(options.TimeoutSeconds, 1, 120));

        services.AddHttpClient<IBusinessCatalogApi, BusinessCatalogApiClient>((sp, client) =>
            {
                var opts = sp.GetRequiredService<IOptions<DownstreamOptions>>().Value;
                client.BaseAddress = new Uri(EnsureTrailingSlash(opts.BusinessUnitsApi));
                client.Timeout = timeout;
            })
            .AddHttpMessageHandler<BearerTokenPropagationHandler>();

        services.AddHttpClient<IUserDirectoryApi, UserDirectoryApiClient>((sp, client) =>
            {
                var opts = sp.GetRequiredService<IOptions<DownstreamOptions>>().Value;
                client.BaseAddress = new Uri(EnsureTrailingSlash(opts.OAuthApi));
                client.Timeout = timeout;
            })
            .AddHttpMessageHandler<BearerTokenPropagationHandler>();

        return services;
    }

    private static string EnsureTrailingSlash(string url)
        => url.EndsWith('/') ? url : url + "/";
}
