using System.Net;
using System.Text.Json;

namespace Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (KeyNotFoundException ex)
        {
            await WriteProblem(ctx, HttpStatusCode.NotFound, "Resource not found", ex.Message);
        }
        catch (ArgumentException ex)
        {
            await WriteProblem(ctx, HttpStatusCode.BadRequest, "Invalid request", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteProblem(ctx, HttpStatusCode.InternalServerError, "Server error", "An unexpected error occurred");
        }
    }

    private static async Task WriteProblem(HttpContext ctx, HttpStatusCode code, string title, string detail)
    {
        ctx.Response.ContentType = "application/json";
        ctx.Response.StatusCode = (int)code;

        var payload = new
        {
            error = new
            {
                title,
                status = (int)code,
                detail,
                traceId = ctx.TraceIdentifier,
                path = ctx.Request.Path.Value
            }
        };

        await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
