using Backend.Application.Common.Interfaces;
using Backend.Application.Users.RequestHandlers.Interfaces;
using Backend.Domain.Users;
using Backend.IntegrationTests.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.IntegrationTests.Users.Repositories;

///<summary>
/// Tests for
/// <see cref="IUsersRepository"/>.
/// </summary>
public class UsersRepositoryTests :
    IClassFixture<ApplicationTestServerFactory>,
    IAsyncLifetime
{
    private readonly ApplicationTestServerFactory _factory;
    private readonly AsyncServiceScope _scope;
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;

    private AppUser _appUser = null!;

    public UsersRepositoryTests(
        ApplicationTestServerFactory factory)
    {
        _factory = factory;
        _scope = _factory.CreateScope();
        _usersRepository =
            _scope.ServiceProvider
                .GetRequiredService<IUsersRepository>();
        _unitOfWork =
            _scope.ServiceProvider
                .GetRequiredService<IUnitOfWork>();
    }

    public async Task InitializeAsync()
    {
        _appUser = AppUser.SyncFromClerk(
            clerkId: "clerk-123",
            username: "anton",
            email: "anton@example.com",
            imageUrl: "https://example.com/avatar.png");
        await _factory.ResetDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task AddUser_ShouldAddUser()
    {
        _usersRepository.AddUser(_appUser);
        await _unitOfWork.SaveChangesAsync(
            CancellationToken.None);

        AppUser? savedUser =
            await _usersRepository
                .GetUserByClerkIdAsync("clerk-123", CancellationToken.None);
        Assert.NotNull(savedUser);
        Assert.Equal(_appUser.Username, savedUser.Username);
        Assert.Equal(_appUser.Email, savedUser.Email);
        Assert.Equal(_appUser.ImageUrl, savedUser.ImageUrl);
    }
}
