using BugKnowledgeBase.Application.DTOs.Dashboard;

namespace BugKnowledgeBase.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default);
}
