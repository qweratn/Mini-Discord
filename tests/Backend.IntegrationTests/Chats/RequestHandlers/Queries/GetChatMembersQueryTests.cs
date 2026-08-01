using Backend.Application.ChatMemberships.Interfaces;
using Backend.Application.Chats.Interfaces;
using Backend.Application.Chats.Models.Responses;
using Backend.Application.Chats.RequestHandlers.Queries;
using Backend.Application.Common.Interfaces;
using Backend.Application.Users.Interfaces;
using Backend.Domain.ChatMemberships;
using Backend.Domain.Chats;
using Backend.Domain.Users;
using Backend.IntegrationTests.Configuration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.IntegrationTests.Chats.RequestHandlers.Queries;

///<summary>
/// Tests for
/// <see cref="GetChatMembersQuery"/>.
/// </summary>
public class GetChatMembersQueryTests :
    IClassFixture<ApplicationTestServerFactory>,
    IAsyncLifetime
{
    private readonly ApplicationTestServerFactory _factory;
    private readonly AsyncServiceScope _scope;
    private readonly IMediator _mediator;
    private readonly IUsersRepository _usersRepository;
    private readonly IChatsRepository _chatsRepository;
    private readonly IChatMembershipsRepository _chatMembershipsRepository;
    private readonly IUnitOfWork _unitOfWork;

    private AppUser _user = null!;
    private Chat _chat = null!;
    private ChatMembership _chatMembership = null!;


    public GetChatMembersQueryTests(
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
            clerkId: "user-clerk-id",
            username: "anton",
            email: "anton@example.com",
            imageUrl: "https://example.com/avatar.png");
        _chat = Chat.CreateServer("Server chat", _user.Id);
        _chatMembership = ChatMembership.Create(_chat.Id, _user.Id);
        _usersRepository.AddUser(_user);
        _chatsRepository.AddChat(_chat);
        _chatMembershipsRepository.AddChatMembership(_chatMembership);
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task HandleGetChatMembers_ShouldReturn()
    {
        IReadOnlyList<ChatMemberInfo> members = await _mediator.Send(
            new GetChatMembersQuery.Query(_chat.Id));

        ChatMemberInfo member = Assert.Single(members);
        Assert.Equal(_user.Id, member.Id);
        Assert.Equal(_user.Username, member.Name);
        Assert.Equal(_user.Email, member.Email);
        Assert.Equal(_user.ImageUrl, member.ImageUrl);
    }
}
