using BugKnowledgeBase.Domain.Common;

namespace BugKnowledgeBase.Domain.Entities;

public class KnowledgeArticle : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    
    public string AuthorSessionName { get; set; } = string.Empty;
    public string? ClassifiedInformation { get; set; }
    public string? TargetStructure { get; set; }
    
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    
    public ICollection<ArticleAttachment> Attachments { get; set; } = new List<ArticleAttachment>();
}
