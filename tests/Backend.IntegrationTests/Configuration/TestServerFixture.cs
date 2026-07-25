using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Backend.IntegrationTests.Configuration;

public abstract class TestServerFixture<TEntryPoint>
    : WebApplicationFactory<TEntryPoint>, IAsyncLifetime
    where TEntryPoint : class
{
    protected virtual string EnvironmentName => "Testing";

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment(EnvironmentName);

        builder.ConfigureAppConfiguration(
            ConfigureApplicationConfiguration);

        builder.ConfigureTestServices(
            ConfigureApplicationServices);

        builder.ConfigureLogging(
            ConfigureApplicationLogging);
    }

    /// <summary>
    /// Allows a derived class to add or override application configuration.
    /// </summary>
    protected virtual void ConfigureApplicationConfiguration(
        WebHostBuilderContext context,
        IConfigurationBuilder configuration)
    {
    }

    /// <summary>
    /// Allows a derived class to replace application services.
    /// </summary>
    protected virtual void ConfigureApplicationServices(
        IServiceCollection services)
    {
    }

    /// <summary>
    /// Configures logging for the test application.
    /// </summary>
    protected virtual void ConfigureApplicationLogging(
        WebHostBuilderContext context,
        ILoggingBuilder logging)
    {
    }

    /// <summary>
    /// Starts Docker containers, message brokers, and other external dependencies.
    /// This method is called before the application is created.
    /// </summary>
    protected virtual Task StartExternalDependenciesAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Applies migrations, seeds the database, and performs other initialization.
    /// The application and its dependency injection container have already been created.
    /// </summary>
    protected virtual Task InitializeApplicationAsync(
        IServiceProvider services)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops external dependencies.
    /// </summary>
    protected virtual Task StopExternalDependenciesAsync()
    {
        return Task.CompletedTask;
    }

    public AsyncServiceScope CreateScope()
    {
        return Services.CreateAsyncScope();
    }

    public async Task InitializeAsync()
    {
        await StartExternalDependenciesAsync();
        await InitializeApplicationAsync(Services);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        Dispose();
        await StopExternalDependenciesAsync();
    }
}
