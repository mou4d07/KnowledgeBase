using BugKnowledgeBase.Domain.Enums;
using BugKnowledgeBase.Application.DTOs.Articles; // Added for ArticleAttachmentDto

namespace BugKnowledgeBase.Application.DTOs.Bugs;

public class BugDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Severity Severity { get; set; }
    public EnvironmentType Environment { get; set; }
    public string? BusinessImpact { get; set; }
    public string? Symptoms { get; set; }
    public string? LogsOrErrorMessages { get; set; }
    public BugStatus Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public string? AssignedToSessionName { get; set; }

    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? ClassifiedInformation { get; set; }
    public string? TargetStructure { get; set; }

    public List<BugKnowledgeBase.Application.DTOs.Common.AttachmentDto> Attachments { get; set; } = new();
}
