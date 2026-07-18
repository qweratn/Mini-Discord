using Backend.Domain.Common;

namespace Backend.Domain.GuildMembership;

/// <summary>
/// Relationship between a user and a guild, representing the user's membership in the guild.
/// </summary>
public class GuildMembership : AggregateRoot
{
    public Guid GuildId { get; private set; }

    public Guid MemberId { get; private set; }

    public DateTimeOffset JoinedAt { get; private set; }

    public GuildMembership()
    {
    }

    public GuildMembership(Guid guildId, Guid memberId)
    {
        GuildId = guildId;
        MemberId = memberId;
        JoinedAt = DateTimeOffset.Now;
    }

    public static GuildMembership Create(Guid guildId, Guid memberId)
    {
        if (guildId == Guid.Empty)
        {
            throw new DomainException("Guild ID cannot be empty.");
        }

        if (memberId == Guid.Empty)
        {
            throw new DomainException("Member ID cannot be empty.");
        }

        GuildMembership guildMembership = new GuildMembership(guildId, memberId);

        guildMembership.AddDomainEvent(
            new GuildMemberJoinedDomainEvent(
                guildMembership.GuildId,
                guildMembership.MemberId,
                guildMembership.JoinedAt));

        return guildMembership;
    }
}
