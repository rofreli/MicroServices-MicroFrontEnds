using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OAuth.Domain.Interfaces;
using OAuth.Infrastructure.OpenIddict;
using OAuth.Infrastructure.Persistence;
using OAuth.Infrastructure.Persistence.Repositories;
using OAuth.Infrastructure.Seeding;
using OAuth.Infrastructure.Services;

namespace OAuth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));
        services.AddSingleton<MongoDbContext>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.Configure<OpenIddictSeederOptions>(configuration.GetSection("OpenIddict:Seeder"));
        services.AddHostedService<OpenIddictSeeder>();

        services.Configure<UserSeederOptions>(configuration.GetSection("Seeding"));
        services.AddHostedService<UserSeeder>();

        return services;
    }
}
