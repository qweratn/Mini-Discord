using Backend.Application.ChatMemberships.Interfaces;
using Backend.Application.Chats.Interfaces;
using Backend.Application.Chats.Models.Responses;
using Backend.Application.Chats.RequestHandlers.Queries;
using Backend.Application.Common.Exceptions;
using Backend.Application.Common.Interfaces;
using Backend.Application.Users.Interfaces;
using Backend.Domain.ChatMemberships;
using Backend.Domain.Chats;
using Backend.Domain.Enums;
using Backend.Domain.Users;
using Backend.IntegrationTests.Configuration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.IntegrationTests.Chats.RequestHandlers.Queries;

///<summary>
/// Tests for
/// <see cref="GetChatInfoQuery"/>.
/// </summary>
public class GetChatInfoQueryTests :
    IClassFixture<ApplicationTestServerFactory>,
    IAsyncLifetime
{
    private const string UserClerkId = "user-clerk-id";
    private const string CompanionClerkId = "companion-clerk-id";
    private const string EmptyUserClerkId = "empty-user-clerk-id";

    private readonly ApplicationTestServerFactory _factory;
    private readonly AsyncServiceScope _scope;
    private readonly IMediator _mediator;
    private readonly IUsersRepository _usersRepository;
    private readonly IChatsRepository _chatsRepository;
    private readonly IChatMembershipsRepository _chatMembershipsRepository;
    private readonly IUnitOfWork _unitOfWork;

    private AppUser _user = null!;
    private AppUser _companion = null!;
    private Chat _serverChat = null!;
    private Chat _directChat = null!;
    private ChatMembership _serverChatMembership = null!;
    private ChatMembership _directUserChatMembership = null!;
    private ChatMembership _directCompanionChatMembership = null!;


    public GetChatInfoQueryTests(
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
        _user = AppUser.SyncFromClerk(
            clerkId: UserClerkId,
            username: "anton",
            email: "anton@example.com",
            imageUrl: "https://example.com/avatar.png");
        _companion = AppUser.SyncFromClerk(
            clerkId: CompanionClerkId,
            username: "companion",
            email: "companion@example.com",
            imageUrl: "https://example.com/avatar.png");
        _serverChat = Chat.CreateServer("Server", _user.Id);
        _directChat = Chat.CreateDirect(_user.Id, _companion.Id);
        _serverChatMembership = ChatMembership.Create(_serverChat.Id, _user.Id);
        _directUserChatMembership = ChatMembership.Create(_directChat.Id, _user.Id);
        _directCompanionChatMembership = ChatMembership.Create(_directChat.Id, _companion.Id);
        _usersRepository.AddUser(_user);
        _usersRepository.AddUser(_companion);
        _chatsRepository.AddChat(_serverChat);
        _chatsRepository.AddChat(_directChat);
        _chatMembershipsRepository.AddChatMembership(_serverChatMembership);
        _chatMembershipsRepository.AddChatMembership(_directUserChatMembership);
        _chatMembershipsRepository.AddChatMembership(_directCompanionChatMembership);
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task HandleGetChatInfo_ServerChat_ShouldReturn()
    {
        ChatInfo info = await _mediator.Send(
            new GetChatInfoQuery.Query(UserClerkId, _serverChat.Id));

        Assert.NotNull(info);
        Assert.Equal(_serverChat.Id, info.Id);
        Assert.Equal(_serverChat.Name, info.Name);
        Assert.Equal(ChatType.Server, info.ChatType);
        Assert.Equal(1, info.MembersCount);
    }

    [Fact]
    public async Task HandleGetChatInfo_DirectChat_ShouldReturn()
    {
        ChatInfo info = await _mediator.Send(
            new GetChatInfoQuery.Query(UserClerkId, _directChat.Id));

        Assert.NotNull(info);
        Assert.Equal(_directChat.Id, info.Id);
        Assert.Equal(_companion.Username, info.Name);
        Assert.Equal(ChatType.Direct, info.ChatType);
        Assert.Equal(_companion.ImageUrl, info.ImageUrl);
        Assert.Equal(2, info.MembersCount);
    }

    [Fact]
    public async Task HandleGetChatInfo_UserNotFound_ShouldThrow()
    {
        string exceptionCode = "user.not_found";
        string exceptionMessage = "User was not found.";

        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _mediator.Send(
                new GetChatInfoQuery.Query(EmptyUserClerkId, _serverChat.Id)));

        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public async Task HandleGetChatInfo_ChatNotFound_ShouldThrow()
    {
        Guid nonExistedChatId = Guid.NewGuid();
        string exceptionCode = "chat.not_found";
        string exceptionMessage = "Chat was not found.";

        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _mediator.Send(
                new GetChatInfoQuery.Query(UserClerkId, nonExistedChatId)));

        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public async Task HandleGetChatInfo_UserNotMember_ShouldThrow()
    {
        string exceptionCode = "chat.access_denied";
        string exceptionMessage = "User is not a member of this chat.";

        ForbiddenException exception = await Assert.ThrowsAsync<ForbiddenException>(() =>
            _mediator.Send(
                new GetChatInfoQuery.Query(CompanionClerkId, _serverChat.Id)));

        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);
    }
}
