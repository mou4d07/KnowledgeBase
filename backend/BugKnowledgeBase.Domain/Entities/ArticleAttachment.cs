using BugKnowledgeBase.Domain.Common;

namespace BugKnowledgeBase.Domain.Entities;

public class ArticleAttachment : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }

    public int KnowledgeArticleId { get; set; }
    public KnowledgeArticle KnowledgeArticle { get; set; } = null!;
}
