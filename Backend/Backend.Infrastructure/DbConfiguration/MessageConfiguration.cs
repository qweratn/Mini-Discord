using Backend.Domain.Chats;
using Backend.Domain.Messages;
using Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.DbConfiguration;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    private const int MaxContentLength = 2000;

    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Content)
            .HasMaxLength(MaxContentLength);

        builder
            .HasOne<Chat>()
            .WithMany()
            .HasForeignKey(x => x.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
