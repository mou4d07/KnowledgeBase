using BugKnowledgeBase.Domain.Interfaces;
using BugKnowledgeBase.Infrastructure.Data;
using BugKnowledgeBase.Infrastructure.Data.Interceptors;
using BugKnowledgeBase.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BugKnowledgeBase.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntityInterceptor>();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            options.AddInterceptors(interceptor);
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddHttpContextAccessor();
        services.AddScoped<BugKnowledgeBase.Application.Interfaces.ICurrentUserService, BugKnowledgeBase.Infrastructure.Services.CurrentUserService>();
        services.AddScoped<BugKnowledgeBase.Application.Interfaces.IFileService, BugKnowledgeBase.Infrastructure.Services.FileService>();

        return services;
    }
}
