namespace BugKnowledgeBase.Application.DTOs.Comments;

public class CommentDto
{
    public int Id { get; set; }
    public int BugId { get; set; }
    public string Content { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class CreateCommentDto
{
    public int BugId { get; set; }
    public string Content { get; set; } = string.Empty;
}
