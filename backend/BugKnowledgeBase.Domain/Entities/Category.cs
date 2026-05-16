using BugKnowledgeBase.Domain.Common;

namespace BugKnowledgeBase.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public int? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<Bug> Bugs { get; set; } = new List<Bug>();
}
