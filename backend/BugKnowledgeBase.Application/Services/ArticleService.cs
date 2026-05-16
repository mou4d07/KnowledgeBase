using AutoMapper;
using BugKnowledgeBase.Application.DTOs.Articles;
using BugKnowledgeBase.Application.Interfaces;
using BugKnowledgeBase.Domain.Entities;
using BugKnowledgeBase.Domain.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BugKnowledgeBase.Application.Services;

public class ArticleService : IArticleService
{
    private readonly IRepository<KnowledgeArticle> _articleRepository;
    private readonly IRepository<AuthorizedUser> _userRepository;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;

    public ArticleService(IRepository<KnowledgeArticle> articleRepository, IRepository<AuthorizedUser> userRepository, IMapper mapper, IAuditService auditService)
    {
        _articleRepository = articleRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _auditService = auditService;
    }

    public async Task<IEnumerable<KnowledgeArticleDto>> GetAllArticlesAsync(string? currentUserSessionName = null, CancellationToken cancellationToken = default)
    {
        var articles = await _articleRepository.Query()
            .Include(a => a.Category)
            .Include(a => a.Attachments)
            .ToListAsync(cancellationToken);
            
        var dtos = _mapper.Map<IEnumerable<KnowledgeArticleDto>>(articles).ToList();
        foreach (var dto in dtos)
        {
            var article = articles.First(a => a.Id == dto.Id);
            await ApplyAccessControlAsync(dto, article, currentUserSessionName, cancellationToken);
        }
        return dtos;
    }

    public async Task<KnowledgeArticleDto?> GetArticleByIdAsync(int id, string? currentUserSessionName = null, CancellationToken cancellationToken = default)
    {
        var article = await _articleRepository.Query()
            .Include(a => a.Category)
            .Include(a => a.Attachments)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            
        if (article == null) return null;
        
        var dto = _mapper.Map<KnowledgeArticleDto>(article);
        await ApplyAccessControlAsync(dto, article, currentUserSessionName, cancellationToken);
        return dto;
    }

    private async Task ApplyAccessControlAsync(KnowledgeArticleDto dto, KnowledgeArticle entity, string? currentUserSessionName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(entity.TargetStructure)) return;

        if (string.IsNullOrEmpty(currentUserSessionName))
        {
            dto.ClassifiedInformation = null;
            return;
        }

        var user = (await _userRepository.GetAsync(u => u.WindowsSessionName == currentUserSessionName, cancellationToken)).FirstOrDefault();
        if (user == null || (user.Role != "Admin" && user.Structure != entity.TargetStructure))
        {
            dto.ClassifiedInformation = null;
        }
    }

    public async Task<KnowledgeArticleDto> CreateArticleAsync(CreateKnowledgeArticleDto dto, string authorSessionName, CancellationToken cancellationToken = default)
    {
        var article = _mapper.Map<KnowledgeArticle>(dto);
        article.AuthorSessionName = authorSessionName;

        var created = await _articleRepository.AddAsync(article, cancellationToken);
        
        await _auditService.LogActionAsync(
            action: "CREATE_ARTICLE",
            entityType: "KnowledgeArticle",
            entityId: created.Id.ToString(),
            oldValues: null,
            newValues: System.Text.Json.JsonSerializer.Serialize(created),
            userId: authorSessionName,
            cancellationToken
        );

        // Re-fetch to include properties like Category
        return await GetArticleByIdAsync(created.Id, authorSessionName, cancellationToken) ?? _mapper.Map<KnowledgeArticleDto>(created);
    }

    public async Task UpdateArticleAsync(UpdateKnowledgeArticleDto dto, string editorSessionName, CancellationToken cancellationToken = default)
    {
        var article = await _articleRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (article == null) throw new InvalidOperationException("Article not found.");

        var oldValues = System.Text.Json.JsonSerializer.Serialize(article);
        
        _mapper.Map(dto, article);
        article.UpdatedBy = editorSessionName;
        await _articleRepository.UpdateAsync(article, cancellationToken);

        await _auditService.LogActionAsync(
            action: "UPDATE_ARTICLE",
            entityType: "KnowledgeArticle",
            entityId: article.Id.ToString(),
            oldValues: oldValues,
            newValues: System.Text.Json.JsonSerializer.Serialize(article),
            userId: editorSessionName,
            cancellationToken
        );
    }

    public async Task DeleteArticleAsync(int id, string editorSessionName, CancellationToken cancellationToken = default)
    {
        var article = await _articleRepository.GetByIdAsync(id, cancellationToken);
        if (article == null) return;

        article.IsDeleted = true; // the soft delete interceptor will actually handle this via the DeleteAsync method
        await _articleRepository.DeleteAsync(article, cancellationToken);

        await _auditService.LogActionAsync(
            action: "DELETE_ARTICLE",
            entityType: "KnowledgeArticle",
            entityId: id.ToString(),
            oldValues: System.Text.Json.JsonSerializer.Serialize(article),
            newValues: null,
            userId: editorSessionName,
            cancellationToken
        );
    }

    public async Task LikeArticleAsync(int id, CancellationToken cancellationToken = default)
    {
        var article = await _articleRepository.GetByIdAsync(id, cancellationToken);
        if (article == null) return;

        article.Likes++;
        await _articleRepository.UpdateAsync(article, cancellationToken);
    }

    public async Task DislikeArticleAsync(int id, CancellationToken cancellationToken = default)
    {
        var article = await _articleRepository.GetByIdAsync(id, cancellationToken);
        if (article == null) return;

        article.Dislikes++;
        await _articleRepository.UpdateAsync(article, cancellationToken);
    }
}
