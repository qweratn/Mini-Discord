using Backend.Application.Chats.Models.Responses;
using Backend.Application.Common.Exceptions;
using Backend.Application.SignalR.Interfaces;
using Backend.Domain.ChatMemberships;
using Backend.Domain.Common;
using Backend.Domain.Users;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Outbox;
using Backend.Presentation.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Backend.Presentation.Outbox;

public class OutboxBackgroundService(
    IServiceScopeFactory scopeFactory,
    OutboxEventSerializer serializer,
    ILogger<OutboxBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await using AsyncServiceScope scope =
                    scopeFactory.CreateAsyncScope();

                ApplicationDbContext db = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                IHubContext<ChatHub, IChatClient> hubContext = scope.ServiceProvider
                    .GetRequiredService<IHubContext<ChatHub, IChatClient>>();

                List<OutboxMessage> messages = await db.OutboxMessages
                    .Where(x => x.ProcessedAtUtc == null)
                    .OrderBy(x => x.OccurredAtUtc)
                    .Take(100)
                    .ToListAsync(stoppingToken);

                foreach (OutboxMessage message in messages)
                {
                    try
                    {
                        IDomainEvent domainEvent = serializer.Deserialize(
                            message.Type,
                            message.Content);

                        switch (domainEvent)
                        {
                            case ChatMemberJoinedDomainEvent chatMemberJoinedEvent:
                                await SendChatMemberJoined(
                                    db,
                                    hubContext,
                                    chatMemberJoinedEvent,
                                    stoppingToken);
                                break;
                            default:
                                logger.LogWarning(
                                    "No handler for outbox message {MessageId} of type {MessageType}.",
                                    message.Id,
                                    message.Type);
                                break;
                        }

                        message.MarkProcessed(DateTime.UtcNow);
                        logger.LogInformation(
                            "Successfully processed outbox message {MessageId}.",
                            message.Id);
                    }
                    catch (Exception ex)
                    {
                        message.MarkFailed(ex, DateTime.UtcNow);
                        logger.LogError(
                            ex,
                            "Failed to process outbox message {MessageId}.",
                            message.Id);
                    }
                }

                await db.SaveChangesAsync(stoppingToken);

                await Task.Delay(
                    TimeSpan.FromSeconds(1),
                    stoppingToken);
            }
        }
        catch (Exception e)
        {
            logger.LogError(
                e, "An error occured while processing outbox messages.");
            throw;
        }
    }

    private async Task SendChatMemberJoined(
        ApplicationDbContext db,
        IHubContext<ChatHub, IChatClient> hubContext,
        ChatMemberJoinedDomainEvent memberJoined,
        CancellationToken cancellationToken)
    {
        AppUser user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == memberJoined.MemberId, cancellationToken) ??
                       throw new NotFoundException(
                           "user.not_found",
                           "User was not found");

        ChatMemberInfo memberInfo = new(
            user.Id,
            user.Username,
            user.Email,
            user.ImageUrl);

        string groupName =
            $"chat:{memberJoined.ChatId:N}";

        await hubContext.Clients
            .Group(groupName)
            .ChatMemberJoined(memberJoined.ChatId, memberInfo);
    }
}
