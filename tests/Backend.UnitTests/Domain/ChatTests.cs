using Backend.Domain.Chats;
using Backend.Domain.Common;
using Backend.Domain.Enums;

namespace Backend.UnitTests.Domain;

///<summary>
/// Tests for
/// <see cref="Chat"/>.
/// </summary>
public class ChatTests
{
    private const string ChatName = "Test Chat";
    private readonly Guid _ownerId = Guid.NewGuid();
    private readonly Guid _companionId = Guid.NewGuid();

    [Fact]
    public void HandleCreateServer_ShouldCreateServer()
    {
        Chat chat = Chat.CreateServer(ChatName, _ownerId);

        Assert.NotNull(chat);
        Assert.Equal(ChatName, chat.Name);
        Assert.Equal(_ownerId, chat.OwnerId);
        Assert.Equal(ChatType.Server, chat.Type);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void HandleCreateServer_NameIsNullOrWhiteSpace_ShouldThrow(string? name)
    {
        string exceptionMessage = "Server name cannot be empty.";

        DomainException exception =
            Assert.Throws<DomainException>(() => Chat.CreateServer(
                name,
                _ownerId));
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public void HandleCreateServer_NameMoreThantMaxLength_ShouldThrow()
    {
        string exceptionMessage = "Server name cannot exceed 64 characters.";
        string name = new string('*', 65);

        DomainException exception =
            Assert.Throws<DomainException>(() => Chat.CreateServer(
                name,
                _ownerId));
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public void HandleCreateServer_OwnerIdIsEmpty_ShouldThrow()
    {
        string exceptionMessage = "Owner is required.";

        DomainException exception =
            Assert.Throws<DomainException>(() => Chat.CreateServer(
                ChatName,
                Guid.Empty));
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public void HandleCreateDirect_ShouldCreateDirect()
    {
        Chat chat = Chat.CreateDirect(_ownerId, _companionId);

        Assert.Null(chat.Name);
        Assert.Null(chat.OwnerId);
        Assert.NotNull(chat.DirectChatKey);
        Assert.Contains(_ownerId.ToString(), chat.DirectChatKey);
        Assert.Contains(_companionId.ToString(), chat.DirectChatKey);
        Assert.Equal(ChatType.Direct, chat.Type);
    }

    [Fact]
    public void HandleCreateDirect_UserIdIsEmpty_ShouldThrow()
    {
        string exceptionMessage = "Users are required.";

        DomainException exception =
            Assert.Throws<DomainException>(() => Chat.CreateDirect(Guid.Empty, _companionId));
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public void HandleCreateDirect_UsersIdAreEqual_ShouldThrow()
    {
        string exceptionMessage = "A direct chat cannot be created with the same user.";

        DomainException exception =
            Assert.Throws<DomainException>(() => Chat.CreateDirect(_companionId, _companionId));
        Assert.Equal(exceptionMessage, exception.Message);
    }
}
