var target = Argument("target", "CI");
var configuration = Argument("configuration", "Release");

FilePath solution = File("./Backend.sln");
FilePath unitTests = File("../tests/Backend.UnitTests/Backend.UnitTests.csproj");
FilePath integrationTests = File("../tests/Backend.IntegrationTests/Backend.IntegrationTests.csproj");

DirectoryPath artifactsDirectory = Directory("../artifacts");
DirectoryPath testResultsDirectory = Directory("../artifacts/test-results");

Task("Clean")
    .Does(() =>
    {
        Information("Cleaning artifacts...");
        EnsureDirectoryExists(artifactsDirectory);
        CleanDirectory(artifactsDirectory);
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        Information("Restoring NuGet packages...");
        DotNetRestore(solution.FullPath);
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        Information("Building application...");
        DotNetBuild(solution.FullPath, new DotNetBuildSettings
        {
            Configuration = configuration,
            NoRestore = true
        });
    });

Task("Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
    {
        Information("Running unit tests...");
        DotNetTest(unitTests.FullPath, new DotNetTestSettings
        {
            Configuration = configuration,
            NoBuild = true,
            NoRestore = true,
            ResultsDirectory = testResultsDirectory,
            Loggers = new[]
            {
                "trx;LogFileName=unit-tests.trx"
            }
        });
    });

Task("Integration-Tests")
    .IsDependentOn("Unit-Tests")
    .Does(() =>
    {
        Information("Running integration tests...");
        DotNetTest(integrationTests.FullPath, new DotNetTestSettings
        {
            Configuration = configuration,
            NoBuild = true,
            NoRestore = true,
            ResultsDirectory = testResultsDirectory,
            Loggers = new[]
            {
                "trx;LogFileName=integration-tests.trx"
            }
        });
    });

Task("CI")
    .IsDependentOn("Integration-Tests");

RunTarget(target);
