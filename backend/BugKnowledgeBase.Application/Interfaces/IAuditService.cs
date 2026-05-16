using BugKnowledgeBase.Domain.Entities;

namespace BugKnowledgeBase.Application.Interfaces;

public interface IAuditService
{
    Task<IEnumerable<AuditLog>> GetRecentAuditLogsAsync(int count, CancellationToken cancellationToken = default);
    Task LogActionAsync(string action, string entityType, string? entityId, string? oldValues, string? newValues, string userId, CancellationToken cancellationToken = default);
}
