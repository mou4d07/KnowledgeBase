using AutoMapper;
using BugKnowledgeBase.Application.DTOs.Bugs;
using BugKnowledgeBase.Application.DTOs.Search;
using BugKnowledgeBase.Application.Interfaces;
using BugKnowledgeBase.Domain.Entities;
using BugKnowledgeBase.Domain.Enums;
using BugKnowledgeBase.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugKnowledgeBase.Application.Services;

public class SearchService : ISearchService
{
    private readonly IRepository<Bug> _bugRepository;
    private readonly IRepository<KnowledgeArticle> _articleRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IMapper _mapper;

    public SearchService(
        IRepository<Bug> bugRepository, 
        IRepository<KnowledgeArticle> articleRepository, 
        IRepository<Category> categoryRepository,
        IMapper mapper)
    {
        _bugRepository = bugRepository;
        _articleRepository = articleRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SearchResultDto>> SearchBugsAsync(string? keyword, int? categoryId, Severity? severity, EnvironmentType? environment, CancellationToken cancellationToken = default)
    {
        var lowerKeyword = keyword?.ToLower();
        var categoryIds = new List<int>();

        if (categoryId.HasValue)
        {
            categoryIds.Add(categoryId.Value);
            // Get all sub-category IDs recursively
            var allCategories = await _categoryRepository.Query()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            
            AddSubCategoryIds(categoryId.Value, allCategories, categoryIds);
        }

        // 1. Search Bugs & Solutions
        var bugQuery = _bugRepository.Query()
            .Include(b => b.Category)
            .Include(b => b.Solutions)
            .Include(b => b.Comments)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(lowerKeyword))
        {
            bugQuery = bugQuery.Where(b => 
                b.Title.ToLower().Contains(lowerKeyword) || 
                b.Description.ToLower().Contains(lowerKeyword) || 
                (b.LogsOrErrorMessages != null && b.LogsOrErrorMessages.ToLower().Contains(lowerKeyword)) ||
                b.Solutions.Any(s => s.Description.ToLower().Contains(lowerKeyword) || s.RootCauseAnalysis.ToLower().Contains(lowerKeyword)) ||
                b.Comments.Any(c => c.Content.ToLower().Contains(lowerKeyword))
            );
        }

        if (categoryId.HasValue)
            bugQuery = bugQuery.Where(b => categoryIds.Contains(b.CategoryId));

        if (severity.HasValue)
            bugQuery = bugQuery.Where(b => b.Severity == severity.Value);

        if (environment.HasValue)
            bugQuery = bugQuery.Where(b => b.Environment == environment.Value);

        var bugs = await bugQuery.ToListAsync(cancellationToken);

        // 2. Search Standalone Articles
        var articleQuery = _articleRepository.Query()
            .Include(a => a.Category)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(lowerKeyword))
        {
            articleQuery = articleQuery.Where(a => 
                a.Title.ToLower().Contains(lowerKeyword) || 
                a.Content.ToLower().Contains(lowerKeyword)
            );
        }

        if (categoryId.HasValue)
            articleQuery = articleQuery.Where(a => categoryIds.Contains(a.CategoryId));

        // Ignore severity and environment for articles as they don't have these fields

        var articles = await articleQuery.ToListAsync(cancellationToken);

        // 3. Map both to SearchResultDto and Combine
        var results = new List<SearchResultDto>();

        foreach (var bug in bugs)
        {
            var solution = bug.Solutions.FirstOrDefault(s => s.Status == SolutionStatus.Validated);
            
            // Prioritize manually set AuthorSessionName, then audit CreatedBy, then "Système"
            string authorName = !string.IsNullOrWhiteSpace(bug.AuthorSessionName) 
                ? bug.AuthorSessionName 
                : (!string.IsNullOrWhiteSpace(bug.CreatedBy) ? bug.CreatedBy : "Système");

            results.Add(new SearchResultDto
            {
                Type = "Bug",
                Id = solution?.Id ?? bug.Id,
                BugId = bug.Id,
                Status = (int)bug.Status,
                Title = bug.Title,
                ContentSnippet = solution?.Description ?? bug.Description,
                CategoryId = bug.CategoryId,
                CategoryName = bug.Category?.Name ?? "N/A",
                CreatedAt = bug.CreatedAt,
                AuthorSessionName = bug.AuthorSessionName,
                CreatedBy = bug.CreatedBy
            });
        }

        foreach (var article in articles)
        {
            results.Add(new SearchResultDto
            {
                Type = "Article",
                Id = article.Id,
                Title = article.Title,
                ContentSnippet = article.Content,
                CategoryId = article.CategoryId,
                CategoryName = article.Category?.Name ?? "N/A",
                CreatedAt = article.CreatedAt,
                AuthorSessionName = article.AuthorSessionName,
                CreatedBy = article.CreatedBy
            });
        }

        return results.OrderByDescending(r => r.CreatedAt);
    }

    private void AddSubCategoryIds(int parentId, List<Category> allCategories, List<int> resultIds)
    {
        var subs = allCategories.Where(c => c.ParentCategoryId == parentId).ToList();
        foreach (var sub in subs)
        {
            resultIds.Add(sub.Id);
            AddSubCategoryIds(sub.Id, allCategories, resultIds);
        }
    }
}
