using BugKnowledgeBase.Domain.Common;

namespace BugKnowledgeBase.Domain.Entities;

public class AuthorizedUser : BaseEntity
{
    public string WindowsSessionName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string Role { get; set; } = "User"; // Admin, Contributor, User
    public string Structure { get; set; } = string.Empty;
}
