using Backend.Application.Chats.Interfaces;
using Backend.Application.Common.Interfaces;
using Backend.Domain.Chats;
using Backend.Domain.Enums;
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
    private readonly IChatsRepository _chatsRepository;
    private readonly IUnitOfWork _unitOfWork;

    private Chat _serverChat = null!;
    private Chat _directChat = null!;
    private readonly Guid _firstUserId = Guid.NewGuid();
    private readonly Guid _secondUserId = Guid.NewGuid();

    public ChatsRepositoryTests(
        ApplicationTestServerFactory factory)
    {
        _factory = factory;
        _scope = _factory.CreateScope();
        _chatsRepository =
            _scope.ServiceProvider
                .GetRequiredService<IChatsRepository>();
        _unitOfWork =
            _scope.ServiceProvider
                .GetRequiredService<IUnitOfWork>();
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        _serverChat = Chat.CreateServer(
            name: "Server Chat",
            ownerId: _firstUserId);
        _directChat = Chat.CreateDirect(
            userId1: _firstUserId,
            userId2: _secondUserId);
        _chatsRepository.AddChat(_directChat);
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
            _firstUserId,
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
            _firstUserId,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(_directChat.Id, result.Id);
        Assert.Equal(_directChat.DirectChatKey, result.DirectChatKey);
        Assert.Equal(ChatType.Direct, result.Type);
    }
}
