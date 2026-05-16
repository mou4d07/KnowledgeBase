using BugKnowledgeBase.Domain.Entities;

namespace BugKnowledgeBase.Application.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(string sessionName, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(string sessionName, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(int notificationId, string sessionName, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(string sessionName, CancellationToken cancellationToken = default);
    Task<Notification> CreateNotificationAsync(string targetSessionName, string title, string message, string? linkUrl, CancellationToken cancellationToken = default);
}
