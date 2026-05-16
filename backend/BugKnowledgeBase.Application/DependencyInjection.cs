using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BugKnowledgeBase.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<BugKnowledgeBase.Application.Mappings.MappingProfile>());
        
        services.AddScoped<BugKnowledgeBase.Application.Interfaces.IBugService, BugKnowledgeBase.Application.Services.BugService>();
        services.AddScoped<BugKnowledgeBase.Application.Interfaces.ICategoryService, BugKnowledgeBase.Application.Services.CategoryService>();
        services.AddScoped<BugKnowledgeBase.Application.Interfaces.IUserService, BugKnowledgeBase.Application.Services.UserService>();
        services.AddScoped<BugKnowledgeBase.Application.Interfaces.ISolutionService, BugKnowledgeBase.Application.Services.SolutionService>();
        services.AddScoped<BugKnowledgeBase.Application.Interfaces.ICommentService, BugKnowledgeBase.Application.Services.CommentService>();
        services.AddScoped<BugKnowledgeBase.Application.Interfaces.ISearchService, BugKnowledgeBase.Application.Services.SearchService>();
        services.AddScoped<BugKnowledgeBase.Application.Interfaces.IDashboardService, BugKnowledgeBase.Application.Services.DashboardService>();
        services.AddScoped<BugKnowledgeBase.Application.Interfaces.IChatService, BugKnowledgeBase.Application.Services.ChatService>();
        services.AddScoped<BugKnowledgeBase.Application.Interfaces.INotificationService, BugKnowledgeBase.Application.Services.NotificationService>();
        services.AddScoped<BugKnowledgeBase.Application.Interfaces.IAuditService, BugKnowledgeBase.Application.Services.AuditService>();
        services.AddScoped<BugKnowledgeBase.Application.Interfaces.IArticleService, BugKnowledgeBase.Application.Services.ArticleService>();
        
        return services;
    }
}
