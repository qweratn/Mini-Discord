using Microsoft.Extensions.DependencyInjection;

namespace Backend.Infrastructure;

/// <summary>
/// Configuration class for setting up infrastructure services.
/// </summary>
public static class Configuration
{
    /// <summary>
    /// Adds infrastructure services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        return services;
    }
}
