using Backend.Application.Chats.Interfaces;
using Backend.Application.Common.Interfaces;
using Backend.Application.Messages.Interfaces;
using Backend.Application.Users.Interfaces;
using Backend.Domain.Chats;
using Backend.Domain.Messages;
using Backend.Domain.Users;
using Backend.IntegrationTests.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.IntegrationTests.Repositories;

///<summary>
/// Tests for
/// <see cref="IMessagesRepository"/>.
/// </summary>
public class MessageRepositoryTests :
    IClassFixture<ApplicationTestServerFactory>,
    IAsyncLifetime
{
    private readonly ApplicationTestServerFactory _factory;
    private readonly AsyncServiceScope _scope;
    private readonly IUsersRepository _usersRepository;
    private readonly IChatsRepository _chatsRepository;
    private readonly IMessagesRepository _messagesRepository;
    private readonly IUnitOfWork _unitOfWork;

    private AppUser _user = null!;
    private Chat _chat = null!;
    private Message _firstMessage = null!;
    private Message _secondMessage = null!;

    public MessageRepositoryTests(
        ApplicationTestServerFactory factory)
    {
        _factory = factory;
        _scope = _factory.CreateScope();
        _usersRepository =
            _scope.ServiceProvider
                .GetRequiredService<IUsersRepository>();
        _messagesRepository =
            _scope.ServiceProvider
                .GetRequiredService<IMessagesRepository>();
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
        _user = AppUser.SyncFromClerk(
            clerkId: "OwnerClerkId",
            username: "anton",
            email: "anton@example.com",
            imageUrl: "https://example.com/avatar.png");
        _chat = Chat.CreateServer("Test", _user.Id);
        _firstMessage = Message.Create(
            "first message",
            _user.Id,
            _chat.Id);
        _secondMessage = Message.Create(
            "second message",
            _user.Id,
            _chat.Id);
        _usersRepository.AddUser(_user);
        _chatsRepository.AddChat(_chat);
        _messagesRepository.AddMessage(_firstMessage);
        _messagesRepository.AddMessage(_secondMessage);
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task HandleGetLastMessages_ShouldReturn()
    {
        IReadOnlyCollection<Guid> chatIds = [_chat.Id];

        IReadOnlyDictionary<Guid, Message> messages =
            await _messagesRepository.GetLastMessagesAsync(chatIds, CancellationToken.None);

        Assert.Single(messages);
        Assert.NotNull(messages[_chat.Id]);
        Message message = messages[_chat.Id];
        Assert.Equal(_secondMessage.Id, message.Id);
        Assert.Equal(_secondMessage.Content, message.Content);
    }

    [Fact]
    public async Task HandleGetLastMessages_ShouldReturnEmpty()
    {
        Guid newChatId = Guid.NewGuid();
        IReadOnlyCollection<Guid> chatIds = [newChatId];

        IReadOnlyDictionary<Guid, Message> messages =
            await _messagesRepository.GetLastMessagesAsync(chatIds, CancellationToken.None);

        Assert.Empty(messages);
    }
}
