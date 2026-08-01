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
/// <see cref="AddChatMemberCommand"/>.
/// </summary>
public class AddChatMemberCommandTests :
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
    private AppUser _other = null!;
    private Chat _serverChat = null!;
    private Chat _directChat = null!;
    private ChatMembership _chatOwnerMembership = null!;

    public AddChatMemberCommandTests(
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
        _other = AppUser.SyncFromClerk(
            clerkId: OtherUserClerkId,
            username: "other",
            email: "other@example.com",
            imageUrl: "https://example.com/avatar.png");
        _serverChat = Chat.CreateServer("ServerChat", _owner.Id);
        _directChat = Chat.CreateDirect(_owner.Id, _other.Id);
        _chatOwnerMembership = ChatMembership.Create(_serverChat.Id, _owner.Id);
        _usersRepository.AddUser(_owner);
        _usersRepository.AddUser(_other);
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
    public async Task HandleAddChatMember_ShouldAdd()
    {
        await _mediator.Send(
            new AddChatMemberCommand.Command(OwnerClerkId, _other.Id, _serverChat.Id));
        IReadOnlyList<ChatMembership> membership = await _chatMembershipsRepository
            .GetByChatIdsAsync([_serverChat.Id], CancellationToken.None);

        Assert.Equal(2, membership.Count);
        ChatMembership addedMembership = Assert.Single(
            membership,
            m => m.MemberId == _other.Id);
        Assert.Equal(_serverChat.Id, addedMembership.ChatId);
        Assert.Equal(_other.Id, addedMembership.MemberId);
    }

    [Fact]
    public async Task HandleAddChatMember_ActorUserNotFound_ShouldThrow()
    {
        string newActorUserId = "new-user-id";
        string exceptionCode = "user.not_found";
        string exceptionMessage = "Actor user was not found.";

        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _mediator.Send(
                new AddChatMemberCommand.Command(newActorUserId, _other.Id, _serverChat.Id)));
        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public async Task HandleAddChatMember_TargetUserNotFound_ShouldThrow()
    {
        Guid newTargetUserId = Guid.NewGuid();
        string exceptionCode = "user.not_found";
        string exceptionMessage = "Target user was not found.";

        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _mediator.Send(
                new AddChatMemberCommand.Command(OwnerClerkId, newTargetUserId, _serverChat.Id)));
        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public async Task HandleAddChatMember_ChatNotFound_ShouldThrow()
    {
        Guid newChatId = Guid.NewGuid();
        string exceptionCode = "chat.not_found";
        string exceptionMessage = "Chat was not found.";

        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _mediator.Send(
                new AddChatMemberCommand.Command(OwnerClerkId, _other.Id, newChatId)));
        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public async Task HandleAddChatMember_DirectChat_ShouldThrow()
    {
        string exceptionCode = "chat.not_supported_type";
        string exceptionMessage = "Direct chat was not support.";

        ConflictException exception = await Assert.ThrowsAsync<ConflictException>(() =>
            _mediator.Send(
                new AddChatMemberCommand.Command(OwnerClerkId, _other.Id, _directChat.Id)));
        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public async Task HandleAddChatMember_UserIsNotOwner_ShouldThrow()
    {
        string exceptionCode = "user.not_owner";
        string exceptionMessage = "Actor user is not a chat owner.";

        ConflictException exception = await Assert.ThrowsAsync<ConflictException>(() =>
            _mediator.Send(
                new AddChatMemberCommand.Command(OtherUserClerkId, _owner.Id, _serverChat.Id)));
        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public async Task HandleAddChatMember_MemberAlreadyJoin_ShouldThrow()
    {
        string exceptionCode = "membership.already_joined";
        string exceptionMessage = "Membership was already successfully joined.";

        ConflictException exception = await Assert.ThrowsAsync<ConflictException>(() =>
            _mediator.Send(
                new AddChatMemberCommand.Command(OwnerClerkId, _owner.Id, _serverChat.Id)));
        Assert.Equal(exceptionCode, exception.Code);
        Assert.Equal(exceptionMessage, exception.Message);
    }
}
