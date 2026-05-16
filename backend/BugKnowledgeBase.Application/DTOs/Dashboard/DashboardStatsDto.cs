using BugKnowledgeBase.Application.DTOs.Articles;
using BugKnowledgeBase.Application.DTOs.Bugs;

namespace BugKnowledgeBase.Application.DTOs.Dashboard;

public class DashboardStatsDto
{
    public int OpenBugsCount { get; set; }
    public int ResolvedBugsCount { get; set; }
    public int CriticalBugsCount { get; set; }
    public int ArticlesCount { get; set; }
    
    // In hours
    public double AverageMttr { get; set; }
    
    public Dictionary<string, int> BugsByCategory { get; set; } = new();
    public Dictionary<string, int> BugsBySeverity { get; set; } = new();
    
    public List<BugDto> RecentBugs { get; set; } = new();
    public List<KnowledgeArticleDto> RecentArticles { get; set; } = new();
}
