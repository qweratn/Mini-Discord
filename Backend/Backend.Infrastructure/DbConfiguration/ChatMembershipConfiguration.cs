using Backend.Domain.ChatMemberships;
using Backend.Domain.Chats;
using Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.DbConfiguration;

public class ChatMembershipConfiguration : IEntityTypeConfiguration<ChatMembership>
{
    public void Configure(EntityTypeBuilder<ChatMembership> builder)
    {
        builder.HasKey(x => new { x.ChatId, x.MemberId });

        builder
            .HasOne<Chat>()
            .WithMany()
            .HasForeignKey(x => x.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
