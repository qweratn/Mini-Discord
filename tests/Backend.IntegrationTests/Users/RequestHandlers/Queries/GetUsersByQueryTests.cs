using Backend.Application.Common.Interfaces;
using Backend.Application.Users.Interfaces;
using Backend.Application.Users.Models;
using Backend.Application.Users.RequestHandlers.Queries;
using Backend.Domain.Users;
using Backend.IntegrationTests.Configuration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.IntegrationTests.Users.RequestHandlers.Queries;

///<summary>
/// Tests for
/// <see cref="GetUsersByQuery"/>.
/// </summary>
public class GetUsersByQueryTests :
    IClassFixture<ApplicationTestServerFactory>,
    IAsyncLifetime
{
    private const string RequesterClerkId = "requester-clerk-id";

    private readonly ApplicationTestServerFactory _factory;
    private readonly AsyncServiceScope _scope;
    private readonly IMediator _mediator;
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;

    private AppUser _requester = null!;
    private AppUser _targetUser = null!;

    public GetUsersByQueryTests(ApplicationTestServerFactory factory)
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
        _requester = AppUser.SyncFromClerk(
            clerkId: RequesterClerkId,
            username: "requester",
            email: "test@example.com",
            imageUrl: "https://example.com/avatar.png");
        _targetUser = AppUser.SyncFromClerk(
            clerkId: "TargetClerkId",
            username: "target",
            email: "test1@example.com",
            imageUrl: "https://example.com/avatar.png");
        _usersRepository.AddUser(_requester);
        _usersRepository.AddUser(_targetUser);
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task GetUsersByQuery_ReturnsCorrectUsers()
    {
        string query = "test";

        IEnumerable<SearchAppUser> result =
            await _mediator.Send(new GetUsersByQuery.Query(RequesterClerkId, query));

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(_targetUser.Id, result.First().Id);
    }

    [Fact]
    public async Task GetUsersByQuery_NoMatches_ReturnsEmpty()
    {
        string query = "nonexistent";

        IEnumerable<SearchAppUser> result =
            await _mediator.Send(new GetUsersByQuery.Query(RequesterClerkId, query));

        Assert.Empty(result);
    }
}
