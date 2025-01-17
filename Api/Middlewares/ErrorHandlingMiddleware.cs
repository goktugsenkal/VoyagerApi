using System.Net;
using System.Text.Json;
using NLog;

namespace Api.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the next middleware in the pipeline and catches any exceptions that might be thrown.
    /// If an exception is thrown, it is handled by calling <see cref="HandleExceptionAsync"/> and writing the exception message 
    /// and status code to the response.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> of the request.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Proceed to the next middleware
        }
        catch (Exception exception)
        {
            // Log the exception with user correlation
            LogExceptionWithUserContext(context, exception);
            await HandleExceptionAsync(context, exception); // Handle response
        }
    }

    /// <summary>
    /// Logs exceptions along with user correlation information if available.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> of the request.</param>
    /// <param name="exception">The exception that occurred.</param>
    private void LogExceptionWithUserContext(HttpContext context, Exception exception)
    {
        var user = context.User.Identity?.IsAuthenticated == true
            ? context.User.Identity.Name
            : "Anonymous";

        Logger.Error(exception, "An exception occurred for user: {User}", user);
    }

    /// <summary>
    /// Handles exceptions by serializing the exception message and status code to JSON 
    /// and writing it to the response.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> of the request.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json"; // Set response content type to JSON

        var statusCode = exception switch
        {
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

        response.StatusCode = statusCode;

        var result = JsonSerializer.Serialize(new
        {
            error = "Server Error",
            message = "An error occurred while processing your request. If the issue persists, please contact support.",
            statusCode = response.StatusCode
        });

        return response.WriteAsync(result); // Write the response
    }
}
