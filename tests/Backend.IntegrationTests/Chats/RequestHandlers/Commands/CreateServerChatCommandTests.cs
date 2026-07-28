using Backend.Application.Chats.Models.Responses;
using Backend.Application.Chats.RequestHandlers.Commands;
using Backend.Application.Common.Exceptions;
using Backend.Application.Common.Interfaces;
using Backend.Application.Users.RequestHandlers.Interfaces;
using Backend.Domain.Enums;
using Backend.Domain.Users;
using Backend.IntegrationTests.Configuration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.IntegrationTests.Chats.RequestHandlers.Commands;

public class CreateServerChatCommandTests :
    IClassFixture<ApplicationTestServerFactory>,
    IAsyncLifetime
{
    private const string ChatName = "TestChat";
    private const string UserClerkId = "clerk-123";

    private readonly ApplicationTestServerFactory _factory;
    private readonly AsyncServiceScope _scope;
    private readonly IMediator _mediator;
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;

    private AppUser _appUser = null!;

    public CreateServerChatCommandTests(
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
            clerkId: UserClerkId,
            username: "anton",
            email: "anton@example.com",
            imageUrl: "https://example.com/avatar.png");
        _usersRepository.AddUser(_appUser);
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task HandleCreateServerChat_ShouldCreateNewServerChat()
    {
        ChatResponse response = await _mediator.Send(
            new CreateServerChatCommand.Command(ChatName, UserClerkId));

        Assert.NotNull(response);
        Assert.Equal(ChatName, response.Name);
        Assert.Equal(ChatType.Server, response.ChatType);
        Assert.Equal(_appUser.Id, response.OwnerId);
    }

    [Fact]
    public async Task HandleCreateServerChat_UserDoesNotExist_ShouldThrow()
    {
        string newUserClerkId = Guid.NewGuid().ToString();
        string exceptionCode = "user.not_found";
        string exceptionMessage = "User was not found.";

        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _mediator.Send(new CreateServerChatCommand.Command(ChatName, newUserClerkId)));

        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);
    }
}
