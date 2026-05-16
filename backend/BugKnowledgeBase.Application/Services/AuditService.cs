using BugKnowledgeBase.Application.Interfaces;
using BugKnowledgeBase.Domain.Entities;
using BugKnowledgeBase.Domain.Interfaces;

namespace BugKnowledgeBase.Application.Services;

public class AuditService : IAuditService
{
    private readonly IRepository<AuditLog> _auditRepository;

    public AuditService(IRepository<AuditLog> auditRepository)
    {
        _auditRepository = auditRepository;
    }

    public async Task<IEnumerable<AuditLog>> GetRecentAuditLogsAsync(int count, CancellationToken cancellationToken = default)
    {
        var logs = await _auditRepository.GetAllAsync(cancellationToken);
        return logs.OrderByDescending(l => l.CreatedAt).Take(count);
    }

    public async Task LogActionAsync(string action, string entityType, string? entityId, string? oldValues, string? newValues, string userId, CancellationToken cancellationToken = default)
    {
        var log = new AuditLog
        {
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            UserId = userId
        };
        await _auditRepository.AddAsync(log, cancellationToken);
    }
}
