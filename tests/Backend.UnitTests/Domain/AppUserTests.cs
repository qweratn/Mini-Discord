using Backend.Domain.Common;
using Backend.Domain.Users;

namespace Backend.UnitTests.Domain;

public class AppUserTests
{
    private const string ClerkId = "user_clerk_id";
    private const string Username = "testuser";
    private const string Email = "test@email.com";
    private const string ImageUrl = "testimageurl.com";

    [Fact]
    public void HandleSyncFormClerk_ShouldSyncNewUser()
    {
        AppUser user = AppUser.SyncFromClerk(
            ClerkId,
            Username,
            Email,
            ImageUrl);

        Assert.Equal(ClerkId, user.ClerkId);
        Assert.Equal(Username, user.Username);
        Assert.Equal(Email, user.Email);
        Assert.Equal(ImageUrl, user.ImageUrl);
    }

    [Fact]
    public void HandleSyncFormClerk_ClerkIdIsNull_ShouldThrow()
    {
        string exceptionMessage = "Click ID cannot be null or empty.";

        DomainException exception = Assert.Throws<DomainException>(() =>
            AppUser.SyncFromClerk(
                null,
                Username,
                Email,
                ImageUrl));
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void HandleSyncFormClerk_UsernameIsNullOrWhiteSpace_ShouldThrow(string? username)
    {
        string exceptionMessage = "Username cannot be empty.";

        DomainException exception = Assert.Throws<DomainException>(() =>
            AppUser.SyncFromClerk(
                ClerkId,
                username,
                Email,
                ImageUrl));
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public void HandleSyncFormClerk_UsernameIsMoreThanMaxLength_ShouldThrow()
    {
        string username = new string('*', 512);
        string exceptionMessage = "Username cannot exceed 32 characters.";

        DomainException exception = Assert.Throws<DomainException>(() =>
            AppUser.SyncFromClerk(
                ClerkId,
                username,
                Email,
                ImageUrl));
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void HandleSyncFormClerk_EmailIsNullOrWhiteSpace_ShouldThrow(string? email)
    {
        string exceptionMessage = "Email cannot be empty.";

        DomainException exception = Assert.Throws<DomainException>(() =>
            AppUser.SyncFromClerk(
                ClerkId,
                Username,
                email,
                ImageUrl));
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Fact]
    public void HandleSyncProfile_ShouldSyncProfile()
    {
        string newUsername = "new_username";
        string newEmail = "newemail@email.com";
        AppUser user = AppUser.SyncFromClerk(
            ClerkId,
            Username,
            Email,
            ImageUrl);

        user.SyncProfile(newUsername, newEmail, ImageUrl);

        Assert.Equal(ClerkId, user.ClerkId);
        Assert.Equal(newUsername, user.Username);
        Assert.Equal(newEmail, user.Email);
        Assert.Equal(ImageUrl, user.ImageUrl);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void HandleSyncProfile_UsernameIsNullOrWhiteSpace_ShouldThrow(string? username)
    {
        string exceptionMessage = "Username cannot be empty.";
        AppUser user = AppUser.SyncFromClerk(
            ClerkId,
            Username,
            Email,
            ImageUrl);

        DomainException exception = Assert.Throws<DomainException>(() =>
            user.SyncProfile(username, Email, ImageUrl));
        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void HandleSyncProfile_EmailIsNullOrWhiteSpace_ShouldThrow(string? email)
    {
        string exceptionMessage = "Email cannot be empty.";
        AppUser user = AppUser.SyncFromClerk(
            ClerkId,
            Username,
            Email,
            ImageUrl);

        DomainException exception = Assert.Throws<DomainException>(() =>
            user.SyncProfile(Username, email, ImageUrl));
        Assert.Equal(exceptionMessage, exception.Message);
    }
}
