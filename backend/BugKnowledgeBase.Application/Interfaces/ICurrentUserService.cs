namespace BugKnowledgeBase.Application.Interfaces;

public interface ICurrentUserService
{
    string? UserSessionName { get; }
}
