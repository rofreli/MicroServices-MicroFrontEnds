using BusinessUnits.Domain.Interfaces;
using BusinessUnits.Infrastructure.Persistence;
using BusinessUnits.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessUnits.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));
        services.AddSingleton<MongoDbContext>();
        services.AddScoped<IBusinessRepository, BusinessRepository>();
        services.AddScoped<IBusinessUnitRepository, BusinessUnitRepository>();
        return services;
    }
}
