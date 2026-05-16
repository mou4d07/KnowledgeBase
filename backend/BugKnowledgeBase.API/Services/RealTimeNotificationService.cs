using BugKnowledgeBase.Application.Interfaces;
using BugKnowledgeBase.API.Hubs;
using BugKnowledgeBase.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace BugKnowledgeBase.API.Services;

public class RealTimeNotificationService : IRealTimeNotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public RealTimeNotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationAsync(string sessionName, Notification notification, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(sessionName).SendAsync("ReceiveNotification", notification, cancellationToken);
    }
}
