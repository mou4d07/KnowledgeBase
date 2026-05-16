using System.Security.Claims;
using BugKnowledgeBase.Application.Interfaces;

namespace BugKnowledgeBase.API.Middleware;

public class WindowsAuthMiddleware
{
    private readonly RequestDelegate _next;

    public WindowsAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserService userService)
    {
        // 0. Skip auth for OPTIONS (CORS preflight) or Swagger UI / static files
        var path = context.Request.Path.Value?.ToLowerInvariant();
        if (context.Request.Method == "OPTIONS" || 
            path == null || 
            path.Contains("/swagger") || 
            path.Contains("/favicon") ||
            path.Contains("/uploads"))
        {
            await _next(context);
            return;
        }

        // 1. In a real IIS Windows Auth environment, context.User.Identity.Name would contain the Windows username
        // Example: "DOMAIN\\mounir.boudmagh"
        // For development/testing purposes, we can also support an override header: X-Windows-User
        
        string? windowsUsername = null;

        // Check if IIS Windows Auth is populated
        if (context.User.Identity is { IsAuthenticated: true } && !string.IsNullOrEmpty(context.User.Identity.Name))
        {
            windowsUsername = context.User.Identity.Name;
        }
        else if (context.Request.Headers.TryGetValue("X-Windows-User", out var headerUser))
        {
            // Fallback for local testing via HTTP header
            windowsUsername = headerUser.ToString();
        }
        else if (context.Request.Query.TryGetValue("access_token", out var accessToken) && !string.IsNullOrEmpty(accessToken))
        {
            // SignalR accessTokenFactory sends the token as ?access_token= on the SSE/WebSocket upgrade
            windowsUsername = accessToken.ToString();
        }
        else if (context.Request.Query.TryGetValue("x-windows-user", out var queryUser))
        {
            // Alternative query param fallback
            windowsUsername = queryUser.ToString();
        }

        if (string.IsNullOrEmpty(windowsUsername))
        {
            // Fallback for everyone: use a public guest account
            windowsUsername = "public_user";
        }

        // Clean up domain prefix if present (e.g., "DISI\\mounir" -> "mounir")
        var sessionName = windowsUsername;
        if (sessionName.Contains('\\'))
        {
            sessionName = sessionName.Split('\\')[1];
        }

        // 2. Validate against Database AuthorizedUsers table
        var user = await userService.GetUserBySessionNameAsync(sessionName);
        
        if (user == null)
        {
            // Automatically register new users as standard 'User'
            user = await userService.CreateUserAsync(new BugKnowledgeBase.Application.DTOs.Users.CreateUserDto
            {
                WindowsSessionName = sessionName,
                DisplayName = sessionName == "public_user" ? "Utilisateur Public" : sessionName,
                Role = "User",
                Structure = "Public"
            });
        }
        else if (!user.IsActive)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync($"Account for '{sessionName}' is deactivated.");
            return;
        }

        // 3. Set the custom ClaimsPrincipal so controllers can access it
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, sessionName),
            new Claim(ClaimTypes.Name, sessionName),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, "WindowsAuth");
        var principal = new ClaimsPrincipal(identity);

        context.User = principal;

        // Continue pipeline
        await _next(context);
    }
}
