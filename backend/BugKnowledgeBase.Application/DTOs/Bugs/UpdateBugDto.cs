using BugKnowledgeBase.Domain.Enums;

namespace BugKnowledgeBase.Application.DTOs.Bugs;

public class UpdateBugDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public Severity Severity { get; set; }
    public EnvironmentType Environment { get; set; }
    
    public string? BusinessImpact { get; set; }
    public string? Symptoms { get; set; }
    public string? LogsOrErrorMessages { get; set; }
    public BugStatus Status { get; set; }
    public string? ClassifiedInformation { get; set; }
    public string? TargetStructure { get; set; }
    public string? AssignedToSessionName { get; set; }
}
