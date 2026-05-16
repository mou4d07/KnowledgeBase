namespace BugKnowledgeBase.Application.DTOs.Users;

public class AuthorizedUserDto
{
    public int Id { get; set; }
    public string WindowsSessionName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Structure { get; set; } = string.Empty;
}
