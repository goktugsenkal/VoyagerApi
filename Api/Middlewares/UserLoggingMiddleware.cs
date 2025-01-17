using NLog;

namespace Api.Middlewares;

public class UserLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public UserLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Get the username (or "Anonymous" if not authenticated)
        var username = context.User.Identity?.IsAuthenticated == true
            ? context.User.Identity.Name
            : "Anonymous";

        // Add user info to the NLog context using ScopeContext
        using (ScopeContext.PushProperty("user", username))
        {
            await _next(context);
        }
    }
}