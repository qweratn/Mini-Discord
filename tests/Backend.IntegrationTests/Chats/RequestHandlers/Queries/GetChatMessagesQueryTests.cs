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

public class GetChatMessagesQueryTests :
    IClassFixture<ApplicationTestServerFactory>,
    IAsyncLifetime
{
    private const string MemberClerkId = "user-clerk-id";
    private const string NotMemberClerkId = "not-member-clerk-id";

    private readonly ApplicationTestServerFactory _factory;
    private readonly AsyncServiceScope _scope;
    private readonly IMediator _mediator;
    private readonly IUsersRepository _usersRepository;
    private readonly IChatsRepository _chatsRepository;
    private readonly IChatMembershipsRepository _chatMembershipsRepository;
    private readonly IMessagesRepository _messagesRepository;
    private readonly IUnitOfWork _unitOfWork;

    private AppUser _member = null!;
    private AppUser _notMember = null!;
    private Chat _chat = null!;
    private Chat _otherChat = null!;
    private ChatMembership _chatMembership = null!;
    private Message _firstMessage = null!;
    private Message _secondMessage = null!;
    private Message _otherChatMessage = null!;


    public GetChatMessagesQueryTests(
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
        _member = AppUser.SyncFromClerk(
            clerkId: MemberClerkId,
            username: "anton",
            email: "anton@example.com",
            imageUrl: "https://example.com/avatar.png");
        _notMember = AppUser.SyncFromClerk(
            clerkId: NotMemberClerkId,
            username: "not-member",
            email: "notmember@example.com",
            imageUrl: "https://example.com/avatar2.png");
        _chat = Chat.CreateServer("Server chat", _member.Id);
        _otherChat = Chat.CreateServer("Server chat", _notMember.Id);
        _chatMembership = ChatMembership.Create(_chat.Id, _member.Id);
        _firstMessage = Message.Create(
            "first message",
            _member.Id,
            _chat.Id);
        _secondMessage = Message.Create(
            "second message",
            _member.Id,
            _chat.Id);
        _otherChatMessage = Message.Create(
            "other chat message",
            _notMember.Id,
            _otherChat.Id);
        _usersRepository.AddUser(_member);
        _usersRepository.AddUser(_notMember);
        _chatsRepository.AddChat(_chat);
        _chatsRepository.AddChat(_otherChat);
        _chatMembershipsRepository.AddChatMembership(_chatMembership);
        _messagesRepository.AddMessage(_firstMessage);
        _messagesRepository.AddMessage(_secondMessage);
        _messagesRepository.AddMessage(_otherChatMessage);
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task HandleGetChatMessages_ShouldReturn()
    {
        GetChatMessageResponse response = await _mediator.Send(
            new GetChatMessagesQuery.Query(
                MemberClerkId,
                _chat.Id,
                null));

        Assert.NotNull(response);
        Assert.Equal(2, response.Items.Count);
        Assert.Equal(_firstMessage.Id, response.Items[1].Id);
        Assert.Equal(_secondMessage.Id, response.Items[0].Id);
    }

    [Fact]
    public async Task HandleGetChatMessages_UserDoesNotExists_ShouldReturn()
    {
        string nonExistClerkId = "non-exist-id";
        string code = "user.not_found";
        string message = "User was not found.";

        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _mediator.Send(
                new GetChatMessagesQuery.Query(
                    nonExistClerkId,
                    _chat.Id,
                    null)));
        Assert.Equal(code, exception.Code);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task HandleGetChatMessages_ChatDoesNotExists_ShouldReturn()
    {
        Guid nonExistChatId = Guid.NewGuid();
        string code = "chat.not_found";
        string message = "Chat was not found.";

        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _mediator.Send(
                new GetChatMessagesQuery.Query(
                    MemberClerkId,
                    nonExistChatId,
                    null)));
        Assert.Equal(code, exception.Code);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task HandleGetChatMessages_UserIsNotMember_ShouldReturn()
    {
        string code = "chat.access_denied";
        string message = "User is not a member of this chat.";

        ForbiddenException exception = await Assert.ThrowsAsync<ForbiddenException>(() =>
            _mediator.Send(
                new GetChatMessagesQuery.Query(
                    NotMemberClerkId,
                    _chat.Id,
                    null)));
        Assert.Equal(code, exception.Code);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task HandleGetChatMessages_CursorMessageInvalid_ShouldReturn()
    {
        string message = "Message cursor is invalid.";

        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _mediator.Send(
                new GetChatMessagesQuery.Query(
                    MemberClerkId,
                    _chat.Id,
                    _otherChatMessage.Id)));
        Assert.Equal(message, exception.Message);
    }
}
