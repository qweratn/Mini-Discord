using Backend.Application.ChatMemberships.Interfaces;
using Backend.Application.Chats.Interfaces;
using Backend.Application.Chats.Models.Responses;
using Backend.Application.Chats.RequestHandlers.Queries;
using Backend.Application.Common.Exceptions;
using Backend.Application.Common.Interfaces;
using Backend.Application.Messages.Interfaces;
using Backend.Application.Users.Interfaces;
using Backend.Domain.ChatMemberships;
using Backend.Domain.Chats;
using Backend.Domain.Messages;
using Backend.Domain.Users;
using Backend.IntegrationTests.Configuration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.IntegrationTests.Chats.RequestHandlers.Queries;

///<summary>
/// Tests for
/// <see cref="GetUserChatsQuery"/>.
/// </summary>
public class GetUserChatsQueryTests :
    IClassFixture<ApplicationTestServerFactory>,
    IAsyncLifetime
{
    private const string UserClerkId = "user-clerk-id";
    private const string EmptyUserClerkId = "empty-user-clerk-id";

    private readonly ApplicationTestServerFactory _factory;
    private readonly AsyncServiceScope _scope;
    private readonly IMediator _mediator;
    private readonly IUsersRepository _usersRepository;
    private readonly IChatsRepository _chatsRepository;
    private readonly IChatMembershipsRepository _chatMembershipsRepository;
    private readonly IMessagesRepository _messagesRepository;
    private readonly IUnitOfWork _unitOfWork;

    private AppUser _user = null!;
    private AppUser _companion = null!;
    private AppUser _emptyUser = null!;
    private Chat _serverChat = null!;
    private Chat _directChat = null!;
    private ChatMembership _serverChatMembership = null!;
    private ChatMembership _directUserChatMembership = null!;
    private ChatMembership _directCompanionChatMembership = null!;
    private Message _serverChatLastMessage = null!;
    private Message _directChatLastMessage = null!;


    public GetUserChatsQueryTests(
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
        _messagesRepository =
            _scope.ServiceProvider
                .GetRequiredService<IMessagesRepository>();
        _unitOfWork =
            _scope.ServiceProvider
                .GetRequiredService<IUnitOfWork>();
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        _user = AppUser.SyncFromClerk(
            clerkId: UserClerkId,
            username: "anton",
            email: "anton@example.com",
            imageUrl: "https://example.com/avatar.png");
        _companion = AppUser.SyncFromClerk(
            clerkId: "companion_clerk_id",
            username: "companion",
            email: "companion@example.com",
            imageUrl: "https://example.com/avatar.png");
        _emptyUser = AppUser.SyncFromClerk(
            clerkId: EmptyUserClerkId,
            username: "empty-user",
            email: "empty@example.com",
            imageUrl: "https://example.com/avatar.png");
        _serverChat = Chat.CreateServer("Server", _user.Id);
        _directChat = Chat.CreateDirect(_user.Id, _companion.Id);
        _serverChatMembership = ChatMembership.Create(_serverChat.Id, _user.Id);
        _directUserChatMembership = ChatMembership.Create(_directChat.Id, _user.Id);
        _directCompanionChatMembership = ChatMembership.Create(_directChat.Id, _companion.Id);
        _serverChatLastMessage = Message.Create(
            "Server last message",
            _user.Id,
            _serverChat.Id);
        _directChatLastMessage = Message.Create(
            "Direct last message",
            _companion.Id,
            _directChat.Id);
        _usersRepository.AddUser(_user);
        _usersRepository.AddUser(_companion);
        _usersRepository.AddUser(_emptyUser);
        _chatsRepository.AddChat(_serverChat);
        _chatsRepository.AddChat(_directChat);
        _chatMembershipsRepository.AddChatMembership(_serverChatMembership);
        _chatMembershipsRepository.AddChatMembership(_directUserChatMembership);
        _chatMembershipsRepository.AddChatMembership(_directCompanionChatMembership);
        _messagesRepository.AddMessage(_serverChatLastMessage);
        _messagesRepository.AddMessage(_directChatLastMessage);
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task HandleGetUserChats_ShouldReturn()
    {
        IReadOnlyList<UserChatListItem> response = await _mediator.Send(
            new GetUserChatsQuery.Query(UserClerkId));

        Assert.NotEmpty(response);
        Assert.Equal(2, response.Count);
        UserChatListItem serverItem = Assert.Single(
            response,
            item => item.ChatId == _serverChat.Id);
        CheckChatListItem(_serverChat, _serverChatLastMessage, serverItem);
        UserChatListItem directItem = Assert.Single(
            response,
            item => item.ChatId == _directChat.Id);
        CheckChatListItem(_directChat, _directChatLastMessage, directItem);
    }

    [Fact]
    public async Task HandleGetUserChats_ShouldReturnEmpty()
    {
        IReadOnlyList<UserChatListItem> response = await _mediator.Send(
            new GetUserChatsQuery.Query(EmptyUserClerkId));

        Assert.Empty(response);
    }

    [Fact]
    public async Task HandleGetUserChats_UserDoesNotExist_ShouldThrow()
    {
        string notExistUserId = "notExistUserId";
        string exceptionCode = "user.not_found";
        string exceptionMessage = "User was not found.";

        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _mediator.Send(new GetUserChatsQuery.Query(notExistUserId)));
        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);

    }

    private void CheckChatListItem(Chat expected, Message expectedMessage, UserChatListItem actual)
    {
        Assert.Equal(expected.Type, actual.ChatType);
        Assert.Equal(expectedMessage.Content, actual.LastMessage);
        Assert.Equal(
            expectedMessage.SendAt,
            actual.LastMessageAt!.Value,
            TimeSpan.FromMicroseconds(1));
    }
}

