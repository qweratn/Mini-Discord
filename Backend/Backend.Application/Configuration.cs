using Microsoft.Extensions.DependencyInjection;

namespace Backend.Application;

public static class Configuration
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        return services;
    }
}
