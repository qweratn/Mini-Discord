using Backend.Application.Common.FluentValidation;
using FluentValidation;
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
        services.AddValidatorsFromAssembly(
            typeof(Configuration).Assembly);

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(
                typeof(Configuration).Assembly);

            configuration.AddOpenBehavior(
                typeof(ValidationBehavior<,>));
        });

        return services;
    }
}
