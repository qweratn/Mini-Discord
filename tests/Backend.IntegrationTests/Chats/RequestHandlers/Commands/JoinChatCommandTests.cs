using Backend.Application.ChatMemberships.Interfaces;
using Backend.Application.Chats.Interfaces;
using Backend.Application.Chats.RequestHandlers.Commands;
using Backend.Application.Common.Exceptions;
using Backend.Application.Common.Interfaces;
using Backend.Application.Users.Interfaces;
using Backend.Domain.ChatMemberships;
using Backend.Domain.Chats;
using Backend.Domain.Users;
using Backend.IntegrationTests.Configuration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.IntegrationTests.Chats.RequestHandlers.Commands;

///<summary>
/// Tests for
/// <see cref="JoinChatCommand"/>.
/// </summary>
public class JoinChatCommandTests :
        IClassFixture<ApplicationTestServerFactory>,
        IAsyncLifetime
{
    private const string OwnerClerkId = "owner_clerk_id";
    private const string AnotherUserClerkId = "another_clerk_id";

    private readonly ApplicationTestServerFactory _factory;
    private readonly AsyncServiceScope _scope;
    private readonly IMediator _mediator;
    private readonly IUsersRepository _usersRepository;
    private readonly IChatsRepository _chatsRepository;
    private readonly IChatMembershipsRepository _chatMembershipsRepository;
    private readonly IUnitOfWork _unitOfWork;

    private AppUser _owner = null!;
    private AppUser _anotherUser = null!;
    private Chat _serverChat = null!;
    private Chat _directChat = null!;
    private ChatMembership _chatOwnerMembership = null!;

    public JoinChatCommandTests(
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
        _anotherUser = AppUser.SyncFromClerk(
            clerkId: AnotherUserClerkId,
            username: "companion",
            email: "companion@example.com",
            imageUrl: "https://example.com/avatar.png");
        _serverChat = Chat.CreateServer("ServerChat", _owner.Id);
        _directChat = Chat.CreateDirect(_owner.Id, _anotherUser.Id);
        _chatOwnerMembership =
            ChatMembership.Create(_serverChat.Id, _owner.Id);
        _usersRepository.AddUser(_owner);
        _usersRepository.AddUser(_anotherUser);
        _chatsRepository.AddChat(_serverChat);
        _chatsRepository.AddChat(_directChat);
        _chatMembershipsRepository.AddChatMembership(_chatOwnerMembership);
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task HandleJoinChat_ShouldReturn()
    {
        await _mediator.Send(
            new JoinChatCommand.Command(
                AnotherUserClerkId,
                _serverChat.Id));
        IReadOnlyList<ChatMembership> memberships =
            await _chatMembershipsRepository.GetByChatIdsAsync(
                [_serverChat.Id],
                CancellationToken.None);

        Assert.Equal(2, memberships.Count);
        ChatMembership addedMembership = Assert.Single(
            memberships,
            membership => membership.MemberId == _anotherUser.Id);
        Assert.Equal(_serverChat.Id, addedMembership.ChatId);
        Assert.Equal(_anotherUser.Id, addedMembership.MemberId);
    }

    [Fact]
    public async Task HandleJoinChat_UserNotFound_ShouldThrow()
    {
        string nonExistedClerkId = "non_existed_clerk_id";
        string exceptionCode = "user.not_found";
        string exceptionMessage = "User was not found.";

        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _mediator.Send(new JoinChatCommand.Command(
                    nonExistedClerkId,
                    _serverChat.Id)));
        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public async Task HandleJoinChat_ChatNotFound_ShouldThrow()
    {
        Guid nonExistedChatId = Guid.NewGuid();
        string exceptionCode = "chat.not_found";
        string exceptionMessage = "Chat was not found.";

        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _mediator.Send(new JoinChatCommand.Command(
                AnotherUserClerkId,
                nonExistedChatId)));
        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public async Task HandleJoinChat_DirectChat_ShouldThrow()
    {
        string exceptionCode = "chat.not_supported_type";
        string exceptionMessage = "Direct chat was not support.";

        ConflictException exception = await Assert.ThrowsAsync<ConflictException>(() =>
            _mediator.Send(new JoinChatCommand.Command(
                AnotherUserClerkId,
                _directChat.Id)));
        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public async Task HandleJoinChat_MemberAlreadyJoined_ShouldThrow()
    {
        string exceptionCode = "membership.already_joined";
        string exceptionMessage = "Membership was already successfully joined.";

        ConflictException exception = await Assert.ThrowsAsync<ConflictException>(() =>
            _mediator.Send(new JoinChatCommand.Command(
                OwnerClerkId,
                _serverChat.Id)));
        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);
    }
}
