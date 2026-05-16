using BugKnowledgeBase.Application;
using BugKnowledgeBase.Infrastructure;
using BugKnowledgeBase.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Increase request body size limit for Base64-embedded images (200MB)
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 209715200; // 200MB
});

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers(options =>
{
    // No global size filter — handled by Kestrel + IIS config
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddScoped<BugKnowledgeBase.Application.Interfaces.IRealTimeNotificationService, BugKnowledgeBase.API.Services.RealTimeNotificationService>();

// CORS for Next.js frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            //.WithOrigins("http://localhost:3000", "http://localhost:3002", "http://localhost:5173", "http://localhost:5174", "https://local.alsolb-dz.com")
            .WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:3002", "http://localhost:5173", "https://local.alsolb-dz.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

app.UsePathBase("/bdc/api"); // Enable prefix mapping for the whole application
app.UseRouting();

// CORS MUST be used after Routing and before Auth
app.UseCors("CorsPolicy");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<BugKnowledgeBase.Infrastructure.Data.AppDbContext>();
        
        // Ensure "admin_dem" exists
        if (!context.AuthorizedUsers.Any(u => u.WindowsSessionName == "admin_dem"))
        {
            context.AuthorizedUsers.Add(new BugKnowledgeBase.Domain.Entities.AuthorizedUser
            {
                WindowsSessionName = "admin_dem",
                DisplayName = "Administrateur Démontration",
                Role = "Admin",
                IsActive = true,
                Structure = "Administration",
                CreatedAt = DateTime.UtcNow
            });
            context.SaveChanges();
        }

        // Seed Hierarchical Categories if empty
        if (!context.Categories.Any())
        {
            var disi = new BugKnowledgeBase.Domain.Entities.Category { Name = "DISI", Description = "Direction" };
            var drh = new BugKnowledgeBase.Domain.Entities.Category { Name = "DRH", Description = "Direction" };
            context.Categories.AddRange(disi, drh);
            context.SaveChanges();

            var infra = new BugKnowledgeBase.Domain.Entities.Category { Name = "INFRASTRUCTURE", ParentCategoryId = disi.Id, Description = "Département" };
            var dev = new BugKnowledgeBase.Domain.Entities.Category { Name = "DEVELOPPEMENT", ParentCategoryId = disi.Id, Description = "Département" };
            context.Categories.AddRange(infra, dev);
            context.SaveChanges();

            var network = new BugKnowledgeBase.Domain.Entities.Category { Name = "Network", ParentCategoryId = infra.Id, Description = "Service" };
            var system = new BugKnowledgeBase.Domain.Entities.Category { Name = "System", ParentCategoryId = infra.Id, Description = "Service" };
            var web = new BugKnowledgeBase.Domain.Entities.Category { Name = "Web", ParentCategoryId = dev.Id, Description = "Service" };
            context.Categories.AddRange(network, system, web);
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
} 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/bdc/api/swagger/v1/swagger.json", "V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseWebSockets();

app.UseHttpsRedirection();

var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

// app.UseCors("CorsPolicy"); // Already moved up

app.UseMiddleware<WindowsAuthMiddleware>();

app.UseAuthorization();

app.MapControllers();
app.MapHub<BugKnowledgeBase.API.Hubs.ChatHub>("/chathub");
app.MapHub<BugKnowledgeBase.API.Hubs.NotificationHub>("/notificationhub");

app.Run();
