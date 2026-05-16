using System.Security.Claims;
using BugKnowledgeBase.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BugKnowledgeBase.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserSessionName => _httpContextAccessor.HttpContext?.User?.Identity?.Name;
}
