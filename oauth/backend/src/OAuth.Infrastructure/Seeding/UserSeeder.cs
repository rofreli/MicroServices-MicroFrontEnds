using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OAuth.Domain.Entities;
using OAuth.Domain.Interfaces;

namespace OAuth.Infrastructure.Seeding;

public class UserSeeder : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly UserSeederOptions _options;
    private readonly ILogger<UserSeeder> _logger;

    public UserSeeder(
        IServiceProvider serviceProvider,
        IOptions<UserSeederOptions> options,
        ILogger<UserSeeder> logger)
        => (_serviceProvider, _options, _logger) = (serviceProvider, options.Value, logger);

    public async Task StartAsync(CancellationToken ct)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        foreach (var seed in _options.Users)
        {
            if (string.IsNullOrWhiteSpace(seed.Email)) continue;

            var existing = await userRepository.GetByEmailAsync(seed.Email, ct);
            if (existing is not null)
            {
                _logger.LogInformation("Seed user {Email} already exists, skipping.", seed.Email);
                continue;
            }

            var hash = passwordHasher.Hash(seed.Password);
            var user = User.Create(seed.Email, seed.FirstName, seed.LastName, hash, seed.IsSuperAdmin);

            await userRepository.AddAsync(user, ct);
            _logger.LogInformation(
                "Seed user {Email} created (superAdmin={IsSuperAdmin}).",
                seed.Email, seed.IsSuperAdmin);
        }
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}

public class UserSeederOptions
{
    public List<SeedUserEntry> Users { get; set; } = new();
}

public class SeedUserEntry
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsSuperAdmin { get; set; } = false;
}
