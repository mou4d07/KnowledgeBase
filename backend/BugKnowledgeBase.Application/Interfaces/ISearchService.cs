using BugKnowledgeBase.Application.DTOs.Search;

namespace BugKnowledgeBase.Application.Interfaces;

public interface ISearchService
{
    Task<IEnumerable<SearchResultDto>> SearchBugsAsync(string? keyword, int? categoryId, Domain.Enums.Severity? severity, Domain.Enums.EnvironmentType? environment, CancellationToken cancellationToken = default);
}
