using BugKnowledgeBase.Domain.Common;
using BugKnowledgeBase.Domain.Enums;

namespace BugKnowledgeBase.Domain.Entities;

public class Bug : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string AuthorSessionName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Severity Severity { get; set; } = Severity.Medium;
    public EnvironmentType Environment { get; set; } = EnvironmentType.Production;
    public string? BusinessImpact { get; set; }
    public string? Symptoms { get; set; }
    public string? LogsOrErrorMessages { get; set; }
    public BugStatus Status { get; set; } = BugStatus.New;
    public string? AssignedToSessionName { get; set; }
    
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public string? ClassifiedInformation { get; set; }
    public string? TargetStructure { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<BugAttachment> Attachments { get; set; } = new List<BugAttachment>();
    public ICollection<Solution> Solutions { get; set; } = new List<Solution>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
