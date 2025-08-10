using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;

namespace PlantTracker.WebApi.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    async ValueTask<bool> IExceptionHandler.TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        logger.LogError(exception, $"Error Occurred. TraceId: {traceId}");
        int statusCode;
        string title;


        switch (exception)
        {
            case ArgumentException:
                statusCode = 400;
                title = $"Invalid Argument Provided {exception.Message}";
                break;
            case UnauthorizedAccessException:
                statusCode = StatusCodes.Status401Unauthorized;
                title = "Unauthorized Access";
                break;
            default:
                statusCode = StatusCodes.Status500InternalServerError;
                title = "Internal Server Error Occurred.";
                break;
        }

        await Results.Problem(
            title: title,
            statusCode: statusCode,
            extensions: new Dictionary<string, object?>()
            {
                { "traceId", traceId },
                { "error", exception.Message }
            }).ExecuteAsync(httpContext);

        return true;
    }
}