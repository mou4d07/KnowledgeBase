using BugKnowledgeBase.Domain.Entities;
using BugKnowledgeBase.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace BugKnowledgeBase.Infrastructure.Data;

public class AppDbContext : DbContext
{
    private readonly AuditableEntityInterceptor _auditableEntityInterceptor;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        AuditableEntityInterceptor auditableEntityInterceptor) : base(options)
    {
        _auditableEntityInterceptor = auditableEntityInterceptor;
    }

    public DbSet<AuthorizedUser> AuthorizedUsers => Set<AuthorizedUser>();
    public DbSet<Bug> Bugs => Set<Bug>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<BugAttachment> BugAttachments => Set<BugAttachment>();
    public DbSet<Solution> Solutions => Set<Solution>();
    public DbSet<SolutionAttachment> SolutionAttachments => Set<SolutionAttachment>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<ChatConversation> ChatConversations => Set<ChatConversation>();
    public DbSet<ChatParticipant> ChatParticipants => Set<ChatParticipant>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<KnowledgeArticle> KnowledgeArticles => Set<KnowledgeArticle>();
    public DbSet<ArticleAttachment> ArticleAttachments => Set<ArticleAttachment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Category hierarchy
        modelBuilder.Entity<Category>(entity => {
            entity.ToTable("Categories");
            entity.HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Global query filter for soft deletes
        modelBuilder.Entity<Bug>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Category>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Solution>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<BugAttachment>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<SolutionAttachment>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Comment>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<AuthorizedUser>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<KnowledgeArticle>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ArticleAttachment>().HasQueryFilter(e => !e.IsDeleted);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntityInterceptor);
    }
}
