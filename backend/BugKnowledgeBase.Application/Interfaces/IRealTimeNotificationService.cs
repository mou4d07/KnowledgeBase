using BugKnowledgeBase.Domain.Entities;

namespace BugKnowledgeBase.Application.Interfaces;

public interface IRealTimeNotificationService
{
    Task SendNotificationAsync(string sessionName, Notification notification, CancellationToken cancellationToken = default);
}
