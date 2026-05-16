using BugKnowledgeBase.Application.DTOs.Articles;

namespace BugKnowledgeBase.Application.Interfaces;

public interface IArticleService
{
    Task<IEnumerable<KnowledgeArticleDto>> GetAllArticlesAsync(string? currentUserSessionName = null, CancellationToken cancellationToken = default);
    Task<KnowledgeArticleDto?> GetArticleByIdAsync(int id, string? currentUserSessionName = null, CancellationToken cancellationToken = default);
    Task<KnowledgeArticleDto> CreateArticleAsync(CreateKnowledgeArticleDto dto, string authorSessionName, CancellationToken cancellationToken = default);
    Task UpdateArticleAsync(UpdateKnowledgeArticleDto dto, string editorSessionName, CancellationToken cancellationToken = default);
    Task DeleteArticleAsync(int id, string editorSessionName, CancellationToken cancellationToken = default);
    Task LikeArticleAsync(int id, CancellationToken cancellationToken = default);
    Task DislikeArticleAsync(int id, CancellationToken cancellationToken = default);
}
