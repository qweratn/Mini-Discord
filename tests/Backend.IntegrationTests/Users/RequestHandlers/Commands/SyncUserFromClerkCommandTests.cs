using Backend.Application.Common.Interfaces;
using Backend.Application.Users.Interfaces;
using Backend.Application.Users.Models;
using Backend.Application.Users.RequestHandlers.Commands;
using Backend.Domain.Users;
using Backend.IntegrationTests.Configuration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.IntegrationTests.Users.RequestHandlers.Commands;

///<summary>
/// Tests for
/// <see cref="SyncUserFromClerkCommand"/>.
/// </summary>
public class SyncUserFromClerkCommandTests :
    IClassFixture<ApplicationTestServerFactory>,
    IAsyncLifetime
{
    private readonly ApplicationTestServerFactory _factory;
    private readonly AsyncServiceScope _scope;
    private readonly IMediator _mediator;
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;

    private AppUser _appUser = null!;
    private UpsertAppUser _upsertAppUser = null!;

    public SyncUserFromClerkCommandTests(
        ApplicationTestServerFactory factory)
    {
        _factory = factory;
        _scope = _factory.CreateScope();
        _mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
        _usersRepository =
            _scope.ServiceProvider
                .GetRequiredService<IUsersRepository>();
        _unitOfWork =
            _scope.ServiceProvider
                .GetRequiredService<IUnitOfWork>();
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        _appUser = AppUser.SyncFromClerk(
            clerkId: "clerk-123",
            username: "anton",
            email: "anton@example.com",
            imageUrl: "https://example.com/avatar.png");
        _upsertAppUser = new UpsertAppUser(
            ClerkId: _appUser.ClerkId,
            Username: "new username",
            Email: "new@example.com",
            ImageUrl: "https://new-example.com/avatar.png");
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task HandleSync_UserIsNull_ShouldSyncFromClerk()
    {
        AppUser? userBeforeSync = await _usersRepository
            .GetUserByClerkIdAsync(_upsertAppUser.ClerkId, CancellationToken.None);

        await _mediator.Send(new SyncUserFromClerkCommand.Command(_upsertAppUser));
        AppUser? userAfterSync = await _usersRepository
            .GetUserByClerkIdAsync(_upsertAppUser.ClerkId, CancellationToken.None);

        Assert.Null(userBeforeSync);
        Assert.NotNull(userAfterSync);
        Assert.Equal(_upsertAppUser.ClerkId, userAfterSync.ClerkId);
        Assert.Equal(_upsertAppUser.Username, userAfterSync.Username);
        Assert.Equal(_upsertAppUser.Email, userAfterSync.Email);
        Assert.Equal(_upsertAppUser.ImageUrl, userAfterSync.ImageUrl);
    }

    [Fact]
    public async Task HandleSync_UserIsNotNull_ShouldSyncProfile()
    {
        _usersRepository.AddUser(_appUser);
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

        await _mediator.Send(new SyncUserFromClerkCommand.Command(_upsertAppUser));
        AppUser? userAfterSync = await _usersRepository
            .GetUserByClerkIdAsync(_upsertAppUser.ClerkId, CancellationToken.None);

        Assert.NotNull(userAfterSync);
        Assert.Equal(_upsertAppUser.ClerkId, userAfterSync.ClerkId);
        Assert.Equal(_upsertAppUser.Username, userAfterSync.Username);
        Assert.Equal(_upsertAppUser.Email, userAfterSync.Email);
        Assert.Equal(_upsertAppUser.ImageUrl, userAfterSync.ImageUrl);
    }
}
