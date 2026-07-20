using Microsoft.Extensions.DependencyInjection;

namespace Backend.Application;

/// <summary>
/// Configuration class for setting up application services.
/// </summary>
public static class Configuration
{
    /// <summary>
    /// Adds application services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        return services;
    }
}
