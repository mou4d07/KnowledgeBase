using BugKnowledgeBase.Domain.Common;
using BugKnowledgeBase.Domain.Enums;

namespace BugKnowledgeBase.Domain.Entities;

public class Solution : BaseEntity
{
    public string RootCauseAnalysis { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Workaround { get; set; }
    
    public SolutionStatus Status { get; set; } = SolutionStatus.Validated;
    
    public int Version { get; set; } = 1;

    public int BugId { get; set; }
    public Bug Bug { get; set; } = null!;

    public ICollection<SolutionAttachment> Attachments { get; set; } = new List<SolutionAttachment>();
}
