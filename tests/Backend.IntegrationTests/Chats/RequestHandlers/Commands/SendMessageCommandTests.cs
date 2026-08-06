using Backend.Application.ChatMemberships.Interfaces;
using Backend.Application.Chats.Interfaces;
using Backend.Application.Chats.Models.Request;
using Backend.Application.Chats.RequestHandlers.Commands;
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

namespace Backend.IntegrationTests.Chats.RequestHandlers.Commands;

///<summary>
/// Tests for
/// <see cref="SendMessageCommand"/>.
/// </summary>
public class SendMessageCommandTests :
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
    private readonly IMessagesRepository _messagesRepository;
    private readonly IUnitOfWork _unitOfWork;

    private AppUser _owner = null!;
    private AppUser _anotherUser = null!;
    private Chat _serverChat = null!;
    private ChatMembership _chatOwnerMembership = null!;
    private Message _message = null!;

    public SendMessageCommandTests(
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
        _chatOwnerMembership =
            ChatMembership.Create(_serverChat.Id, _owner.Id);
        _message = Message.Create(
            content: "Hello, world!",
            authorId: _owner.Id,
            chatId: _serverChat.Id);
        _usersRepository.AddUser(_owner);
        _usersRepository.AddUser(_anotherUser);
        _chatsRepository.AddChat(_serverChat);
        _chatMembershipsRepository.AddChatMembership(_chatOwnerMembership);
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task HandleSendMessage_ShouldSend()
    {
        SendMessage message = new SendMessage(
           "Hello, world!",
            OwnerClerkId,
            _serverChat.Id);

        await _mediator.Send(new SendMessageCommand.Command(message));
        IReadOnlyDictionary<Guid, Message> lastMessages =
            await _messagesRepository.GetLastMessagesAsync([_serverChat.Id], CancellationToken.None);
        Message lastMessage = lastMessages.Values.First();

        Assert.Equal(message.Content, lastMessage.Content);
        Assert.Equal(_owner.Id, lastMessage.AuthorId);
        Assert.Equal(_serverChat.Id, lastMessage.ChatId);
    }

    [Fact]
    public async Task HandleSendMessage_UserNotFound_ShouldThrow()
    {
        SendMessage message = new SendMessage(
            "Hello, world!",
            "NonExistentClerkId",
            _serverChat.Id);
        string exceptionCode = "user.not_found";
        string exceptionMessage = "Author user was not found.";

        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _mediator.Send(new SendMessageCommand.Command(message)));
        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public async Task HandleSendMessage_ChatNotFound_ShouldThrow()
    {
        SendMessage message = new SendMessage(
            "Hello, world!",
            OwnerClerkId,
            Guid.NewGuid());
        string exceptionCode = "chat.not_found";
        string exceptionMessage = "Chat was not found.";

        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _mediator.Send(new SendMessageCommand.Command(message)));
        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public async Task HandleSendMessage_AuthorIsNotAMember_ShouldThrow()
    {
        SendMessage message = new SendMessage(
            "Hello, world!",
            AnotherUserClerkId,
            _serverChat.Id);
        string exceptionCode = "user.not_member";
        string exceptionMessage = "Author is not a member of this chat.";

        ConflictException exception = await Assert.ThrowsAsync<ConflictException>(() =>
            _mediator.Send(new SendMessageCommand.Command(message)));
        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);
    }
}
