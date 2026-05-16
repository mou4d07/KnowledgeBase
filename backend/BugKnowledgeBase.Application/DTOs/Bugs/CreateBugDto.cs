using BugKnowledgeBase.Domain.Enums;

namespace BugKnowledgeBase.Application.DTOs.Bugs;

public class CreateBugDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public Severity Severity { get; set; } = Severity.Medium;
    public EnvironmentType Environment { get; set; } = EnvironmentType.Production;
    
    public string? BusinessImpact { get; set; }
    public string? Symptoms { get; set; }
    public string? LogsOrErrorMessages { get; set; }
    public string? ClassifiedInformation { get; set; }
    public string? TargetStructure { get; set; }
}
