using Backend.Domain.Chats;
using Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.DbConfiguration;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    private const int MaxServerNameLength = 64;

    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Name)
            .HasMaxLength(MaxServerNameLength);

        builder
            .Property(x => x.Type)
            .HasConversion<string>();

        builder
            .HasIndex(x => x.DirectChatKey)
            .IsUnique();

        builder
            .HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
