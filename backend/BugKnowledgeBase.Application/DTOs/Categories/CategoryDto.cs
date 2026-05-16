namespace BugKnowledgeBase.Application.DTOs.Categories;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    
    public List<CategoryDto> SubCategories { get; set; } = new();
}
