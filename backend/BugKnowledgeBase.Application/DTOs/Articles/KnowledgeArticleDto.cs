namespace BugKnowledgeBase.Application.DTOs.Articles;

public class KnowledgeArticleDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorSessionName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public string? ClassifiedInformation { get; set; }
    public string? TargetStructure { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public ICollection<ArticleAttachmentDto> Attachments { get; set; } = new List<ArticleAttachmentDto>();
}

public class ArticleAttachmentDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}

public class CreateKnowledgeArticleDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string? ClassifiedInformation { get; set; }
    public string? TargetStructure { get; set; }
}

public class UpdateKnowledgeArticleDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string? ClassifiedInformation { get; set; }
    public string? TargetStructure { get; set; }
}
