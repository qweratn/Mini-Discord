using Backend.Domain.Common;
using Backend.Domain.Messages;

namespace Backend.UnitTests.Domain;

public class MessageTests
{
    private const string Content = "Hello world";
    private readonly Guid _authorId = Guid.NewGuid();
    private readonly Guid _chatId = Guid.NewGuid();

    [Fact]
    public void HandleCreate_ShouldReturnNewMessage()
    {
        Message message = Message.Create(Content, _authorId, _chatId);

        Assert.Equal(Content, message.Content);
        Assert.Equal(_authorId, message.AuthorId);
        Assert.Equal(_chatId, message.ChatId);
        Assert.NotEqual(Guid.Empty, message.Id);
        Assert.True(message.SendAt <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void HandleCreate_EmptyAuthorId_ShouldThrow()
    {
        string exceptionMessage = "Author is required.";

        DomainException exception =
            Assert.Throws<DomainException>(() => Message.Create(
                Content,
                Guid.Empty,
                _chatId));
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public void HandleCreate_EmptyChatId_ShouldThrow()
    {
        string exceptionMessage = "Chat is required.";

        DomainException exception =
            Assert.Throws<DomainException>(() => Message.Create(
                Content,
                _authorId,
                Guid.Empty));
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void HandleCreate_MessageIsNullOrEmpty_ShouldThrow(string? content)
    {
        string exceptionMessage = "Message cannot be empty.";

        DomainException exception =
            Assert.Throws<DomainException>(() => Message.Create(
                content,
                _authorId,
                _chatId));
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public void HandleCreate_ContentMoreThanMaxLength_ShouldThrow()
    {
        string content = new string('a', 2001);
        string exceptionMessage = $"Message cannot exceed 2000 characters.";

        DomainException exception =
            Assert.Throws<DomainException>(() => Message.Create(
                content,
                _authorId,
                _chatId));
        Assert.Equal(exceptionMessage, exception.Message);
    }
}
