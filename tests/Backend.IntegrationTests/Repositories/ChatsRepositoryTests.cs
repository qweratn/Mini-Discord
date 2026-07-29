using Backend.Application.ChatMemberships.Interfaces;
using Backend.Application.Chats.Interfaces;
using Backend.Application.Common.Interfaces;
using Backend.Application.Users.Interfaces;
using Backend.Domain.ChatMemberships;
using Backend.Domain.Chats;
using Backend.Domain.Enums;
using Backend.Domain.Users;
using Backend.IntegrationTests.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.IntegrationTests.Repositories;

///<summary>
/// Tests for
/// <see cref="IChatsRepository"/>.
/// </summary>
public class ChatsRepositoryTests :
    IClassFixture<ApplicationTestServerFactory>,
    IAsyncLifetime
{
    private readonly ApplicationTestServerFactory _factory;
    private readonly AsyncServiceScope _scope;
    private readonly IUsersRepository _usersRepository;
    private readonly IChatsRepository _chatsRepository;
    private readonly IChatMembershipsRepository _chatMembershipsRepository;
    private readonly IUnitOfWork _unitOfWork;

    private Chat _serverChat = null!;
    private Chat _directChat = null!;
    private AppUser _firstUser = null!;
    private readonly Guid _secondUserId = Guid.NewGuid();
    private ChatMembership _directChatMembership = null!;

    public ChatsRepositoryTests(
        ApplicationTestServerFactory factory)
    {
        _factory = factory;
        _scope = _factory.CreateScope();
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
        _firstUser = AppUser.SyncFromClerk(
            clerkId: "OwnerClerkId",
            username: "anton",
            email: "anton@example.com",
            imageUrl: "https://example.com/avatar.png");
        _serverChat = Chat.CreateServer(
            name: "Server Chat",
            ownerId: _firstUser.Id);
        _directChat = Chat.CreateDirect(
            userId1: _firstUser.Id,
            userId2: _secondUserId);
        _usersRepository.AddUser(_firstUser);
        _chatsRepository.AddChat(_directChat);
        _directChatMembership = ChatMembership.Create(_directChat.Id, _firstUser.Id);
        _chatMembershipsRepository.AddChatMembership(_directChatMembership);
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task HandleGetDirectChatByKey_ShouldReturn()
    {
        Chat? chat = await _chatsRepository.GetDirectChatByKey(
            _firstUser.Id,
            _secondUserId,
            CancellationToken.None);

        Assert.NotNull(chat);
        Assert.Equal(_directChat.Id, chat.Id);
        Assert.Equal(_directChat.DirectChatKey, chat.DirectChatKey);
        Assert.Equal(ChatType.Direct, chat.Type);
    }

    [Fact]
    public async Task GetDirectChatByKey_ReversedUserOrder_ShouldReturnExpectedChat()
    {
        Chat? result = await _chatsRepository.GetDirectChatByKey(
            _secondUserId,
            _firstUser.Id,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(_directChat.Id, result.Id);
        Assert.Equal(_directChat.DirectChatKey, result.DirectChatKey);
        Assert.Equal(ChatType.Direct, result.Type);
    }

    [Fact]
    public async Task GetUserChatsAsync_ShouldReturn()
    {
        IReadOnlyList<Chat> chats = await _chatsRepository
            .GetUserChatsAsync(_firstUser.Id, CancellationToken.None);

        Assert.NotNull(chats);
        Assert.Single(chats);
        Chat chat = chats.First();
        Assert.Equal(_directChat.Id, chat.Id);
        Assert.Equal(_directChat.DirectChatKey, chat.DirectChatKey);
    }
}
