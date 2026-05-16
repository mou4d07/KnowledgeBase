using BugKnowledgeBase.Application.DTOs.Articles;
using BugKnowledgeBase.Application.DTOs.Bugs;
using BugKnowledgeBase.Application.DTOs.Dashboard;
using BugKnowledgeBase.Application.Interfaces;
using BugKnowledgeBase.Domain.Entities;
using BugKnowledgeBase.Domain.Enums;
using BugKnowledgeBase.Domain.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace BugKnowledgeBase.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IRepository<Bug> _bugRepository;
    private readonly IRepository<KnowledgeArticle> _articleRepository;

    public DashboardService(IRepository<Bug> bugRepository, IRepository<KnowledgeArticle> articleRepository)
    {
        _bugRepository = bugRepository;
        _articleRepository = articleRepository;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
    {
        var allBugs = await _bugRepository.Query()
            .Include(b => b.Category)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var stats = new DashboardStatsDto
        {
            OpenBugsCount = allBugs.Count(b => b.Status != BugStatus.Resolved && b.Status != BugStatus.Rejected),
            ResolvedBugsCount = allBugs.Count(b => b.Status == BugStatus.Resolved),
            CriticalBugsCount = allBugs.Count(b => b.Severity == Severity.Critical && b.Status != BugStatus.Resolved),
        };

        var resolvedBugs = allBugs.Where(b => b.Status == BugStatus.Resolved && b.ResolvedAt.HasValue).ToList();
        if (resolvedBugs.Any())
        {
            var totalHours = resolvedBugs.Sum(b => (b.ResolvedAt!.Value - b.CreatedAt).TotalHours);
            stats.AverageMttr = Math.Round(totalHours / resolvedBugs.Count, 2);
        }

        stats.BugsByCategory = allBugs.Where(b => b.Category != null)
                                      .GroupBy(b => b.Category.Name)
                                      .ToDictionary(g => g.Key, g => g.Count());

        stats.BugsBySeverity = allBugs.GroupBy(b => b.Severity.ToString())
                                      .ToDictionary(g => g.Key, g => g.Count());

        stats.RecentBugs = allBugs.OrderByDescending(b => b.CreatedAt)
                                  .Take(5)
                                  .Select(b => new BugDto
                                  {
                                      Id = b.Id,
                                      Title = b.Title,
                                      Severity = b.Severity,
                                      Status = b.Status,
                                      CreatedAt = b.CreatedAt,
                                      CreatedBy = b.CreatedBy
                                  }).ToList();

        var articlesQuery = _articleRepository.Query()
                                               .Include(a => a.Category)
                                               .AsNoTracking();

        stats.ArticlesCount = await articlesQuery.CountAsync(cancellationToken);
        
        var recentArticles = await articlesQuery.OrderByDescending(a => a.CreatedAt)
                                               .Take(5)
                                               .ToListAsync(cancellationToken);

        stats.RecentArticles = recentArticles.Select(a => new KnowledgeArticleDto
                                           {
                                               Id = a.Id,
                                               Title = a.Title,
                                               AuthorSessionName = a.AuthorSessionName,
                                               CreatedAt = a.CreatedAt,
                                               CategoryName = a.Category?.Name ?? "N/A"
                                           }).ToList();

        return stats;
    }
}
