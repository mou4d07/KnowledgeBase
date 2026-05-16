using BugKnowledgeBase.Application.Interfaces;
using BugKnowledgeBase.Domain.Entities;
using BugKnowledgeBase.Domain.Interfaces;

namespace BugKnowledgeBase.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IRepository<Notification> _notificationRepository;
    private readonly IRealTimeNotificationService _realTimeNotificationService;

    public NotificationService(
        IRepository<Notification> notificationRepository,
        IRealTimeNotificationService realTimeNotificationService)
    {
        _notificationRepository = notificationRepository;
        _realTimeNotificationService = realTimeNotificationService;
    }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string sessionName, CancellationToken cancellationToken = default)
    {
        return await _notificationRepository.GetAsync(n => n.TargetSessionName == sessionName, cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(string sessionName, CancellationToken cancellationToken = default)
    {
        var unread = await _notificationRepository.GetAsync(n => n.TargetSessionName == sessionName && !n.IsRead, cancellationToken);
        return unread.Count;
    }

    public async Task MarkAsReadAsync(int notificationId, string sessionName, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);
        if (notification != null && notification.TargetSessionName == sessionName)
        {
            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification, cancellationToken);
        }
    }

    public async Task MarkAllAsReadAsync(string sessionName, CancellationToken cancellationToken = default)
    {
        var unread = await _notificationRepository.GetAsync(n => n.TargetSessionName == sessionName && !n.IsRead, cancellationToken);
        foreach (var notification in unread)
        {
            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification, cancellationToken);
        }
    }

    public async Task<Notification> CreateNotificationAsync(string targetSessionName, string title, string message, string? linkUrl, CancellationToken cancellationToken = default)
    {
        var n = new Notification
        {
            TargetSessionName = targetSessionName,
            Title = title,
            Message = message,
            LinkUrl = linkUrl,
            IsRead = false
        };
        var created = await _notificationRepository.AddAsync(n, cancellationToken);

        // Send real-time notification
        await _realTimeNotificationService.SendNotificationAsync(targetSessionName, created, cancellationToken);

        return created;
    }
}
