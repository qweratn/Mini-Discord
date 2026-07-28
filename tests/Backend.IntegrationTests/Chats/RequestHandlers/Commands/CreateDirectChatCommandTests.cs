using Backend.Application.ChatMemberships.Interfaces;
using Backend.Application.Chats.Interfaces;
using Backend.Application.Chats.Models.Responses;
using Backend.Application.Chats.RequestHandlers.Commands;
using Backend.Application.Common.Exceptions;
using Backend.Application.Common.Interfaces;
using Backend.Application.Users.RequestHandlers.Interfaces;
using Backend.Domain.ChatMemberships;
using Backend.Domain.Chats;
using Backend.Domain.Enums;
using Backend.Domain.Users;
using Backend.IntegrationTests.Configuration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.IntegrationTests.Chats.RequestHandlers.Commands;

///<summary>
/// Tests for
/// <see cref="CreateDirectChatCommand"/>.
/// </summary>
public class CreateDirectChatCommandTests :
    IClassFixture<ApplicationTestServerFactory>,
    IAsyncLifetime
{
    private const string OwnerClerkId = "owner_clerk_id";
    private const string OtherUserClerkId = "other_clerk_id";

    private readonly ApplicationTestServerFactory _factory;
    private readonly AsyncServiceScope _scope;
    private readonly IMediator _mediator;
    private readonly IUsersRepository _usersRepository;
    private readonly IChatsRepository _chatsRepository;
    private readonly IChatMembershipsRepository _chatMembershipsRepository;
    private readonly IUnitOfWork _unitOfWork;

    private AppUser _owner = null!;
    private AppUser _companion = null!;
    private AppUser _otherUser = null!;
    private Chat _existingChat = null!;
    private ChatMembership _existingChatOwnerMembership = null!;
    private ChatMembership _existingChatCompanionMembership = null!;

    public CreateDirectChatCommandTests(
        ApplicationTestServerFactory factory)
    {
        _factory = factory;
        _scope = _factory.CreateScope();
        _mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
        _usersRepository =
            _scope.ServiceProvider
                .GetRequiredService<IUsersRepository>();
        _chatsRepository =
            _scope.ServiceProvider
                .GetRequiredService<IChatsRepository>();
        _chatMembershipsRepository =
            _scope.ServiceProvider
                .GetRequiredService<IChatMembershipsRepository>();
        _unitOfWork =
            _scope.ServiceProvider
                .GetRequiredService<IUnitOfWork>();
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        _owner = AppUser.SyncFromClerk(
            clerkId: OwnerClerkId,
            username: "anton",
            email: "anton@example.com",
            imageUrl: "https://example.com/avatar.png");
        _companion = AppUser.SyncFromClerk(
            clerkId: "companion_clerk_id",
            username: "companion",
            email: "companion@example.com",
            imageUrl: "https://example.com/avatar.png");
        _otherUser = AppUser.SyncFromClerk(
            clerkId: OtherUserClerkId,
            username: "other",
            email: "other@example.com",
            imageUrl: "https://example.com/avatar.png");
        _usersRepository.AddUser(_owner);
        _usersRepository.AddUser(_companion);
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);
        _existingChat = Chat.CreateDirect(_owner.Id, _companion.Id);
        _existingChatOwnerMembership =
            ChatMembership.Create(_existingChat.Id, _owner.Id);
        _existingChatCompanionMembership =
            ChatMembership.Create(_existingChat.Id, _companion.Id);
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task HandleCreateDirectChat_DirectChatDoesNotExist_ShouldCreateNew()
    {
        ChatResponse response = await _mediator
            .Send(new CreateDirectChatCommand.Command(OwnerClerkId, _companion.Id));

        Assert.NotNull(response);
        Assert.Null(response.Name);
        Assert.Equal(ChatType.Direct, response.ChatType);
        Assert.Null(response.OwnerId);
    }

    [Fact]
    public async Task HandleCreateDirectChat_DirectChatExists_ShouldReturn()
    {
        _chatsRepository.AddChat(_existingChat);
        _chatMembershipsRepository.AddChatMembership(_existingChatOwnerMembership);
        _chatMembershipsRepository.AddChatMembership(_existingChatCompanionMembership);
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

        ChatResponse response = await _mediator
            .Send(new CreateDirectChatCommand.Command(OwnerClerkId, _companion.Id));

        Assert.NotNull(response);
        Assert.Equal(_existingChat.Id, response.Id);
        Assert.Equal(ChatType.Direct, response.ChatType);
    }

    [Fact]
    public async Task HandleCreateDirectChat_OwnerDoesNotExist_ShouldThrow()
    {
        string exceptionCode = "user.not_found";
        string exceptionMessage = "Owner was not found.";

        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _mediator.Send(new CreateDirectChatCommand.Command(OtherUserClerkId, _companion.Id)));
        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public async Task HandleCreateDirectChat_CompanionDoesNotExist_ShouldThrow()
    {
        string exceptionCode = "user.not_found";
        string exceptionMessage = "Companion user was not found.";

        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _mediator.Send(new CreateDirectChatCommand.Command(OwnerClerkId, _otherUser.Id)));
        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);
    }
}
