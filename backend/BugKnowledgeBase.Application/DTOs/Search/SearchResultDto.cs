namespace BugKnowledgeBase.Application.DTOs.Search;

public class SearchResultDto
{
    // Common fields
    public string Type { get; set; } = string.Empty; // "Solution" or "Article"
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ContentSnippet { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Bug specific
    public int? BugId { get; set; }
    public int? Status { get; set; }
    
    // Article/User specific
    public string? AuthorSessionName { get; set; }
    public string? CreatedBy { get; set; }
}
