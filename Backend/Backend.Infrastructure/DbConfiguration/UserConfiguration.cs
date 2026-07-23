using Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.DbConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .HasIndex(x => x.ClerkId)
            .IsUnique();

        builder
            .HasIndex(x => x.Username)
            .IsUnique();
    }
}
