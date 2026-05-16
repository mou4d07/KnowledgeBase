using BugKnowledgeBase.Domain.Enums;

namespace BugKnowledgeBase.Application.DTOs.Solutions;

public class SolutionDto
{
    public int Id { get; set; }
    public string RootCauseAnalysis { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Workaround { get; set; }
    public SolutionStatus Status { get; set; }
    public int Version { get; set; }
    public int BugId { get; set; }
    public string? BugTitle { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public List<BugKnowledgeBase.Application.DTOs.Common.AttachmentDto> Attachments { get; set; } = new();
}

public class CreateSolutionDto
{
    public int BugId { get; set; }
    public string RootCauseAnalysis { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Workaround { get; set; }
}

public class UpdateSolutionDto
{
    public int Id { get; set; }
    public string RootCauseAnalysis { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Workaround { get; set; }
    public BugKnowledgeBase.Domain.Enums.SolutionStatus Status { get; set; }
}
