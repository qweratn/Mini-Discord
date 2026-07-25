using Backend.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Testcontainers.PostgreSql;

namespace Backend.IntegrationTests.Configuration;

public class ApplicationTestServerFactory
    : TestServerFixture<Program>
{
    private readonly PostgreSqlContainer postgresContainer =
        new PostgreSqlBuilder("postgres:17-alpine")
            .WithDatabase("mini_discord_tests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    private Respawner respawner = null!;

    protected override Task StartExternalDependenciesAsync()
    {
        return postgresContainer.StartAsync();
    }

    protected override void ConfigureApplicationConfiguration(
        WebHostBuilderContext context,
        IConfigurationBuilder configuration)
    {
        configuration.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgreSql"] =
                    postgresContainer.GetConnectionString(),
            });
    }

    protected override async Task InitializeApplicationAsync(
        IServiceProvider services)
    {
        await using AsyncServiceScope scope =
            services.CreateAsyncScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync();

        await using var connection =
            new NpgsqlConnection(
                postgresContainer.GetConnectionString());

        await connection.OpenAsync();

        respawner = await Respawner.CreateAsync(
            connection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["public"],
                TablesToIgnore =
                [
                    new Table("__EFMigrationsHistory"),
                ],
            });
    }

    public async Task ResetDatabaseAsync()
    {
        await using var connection =
            new NpgsqlConnection(
                postgresContainer.GetConnectionString());

        await connection.OpenAsync();
        await respawner.ResetAsync(connection);
    }

    protected override async Task StopExternalDependenciesAsync()
    {
        await postgresContainer.DisposeAsync();
    }
}
