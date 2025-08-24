using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PlantTracker.Core.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;

namespace PlantTracker.WebApi.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    async ValueTask<bool> IExceptionHandler.TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        logger.LogError(exception, $"Error Occurred. TraceId: {traceId}");

        var problemDetails = exception switch
        {
            JsonException => new ProblemDetails
            {
                Title = "Invalid Request Body Format",
                Detail = "The submitted data is malformed or does not match the expected structure.",
                Status = StatusCodes.Status400BadRequest
            },
            ArgumentException => new ProblemDetails
            {
                Status = 400,
                Title = "Invalid Argument Provided",
                Detail = exception.Message
            },
            ValidationException => new ProblemDetails
            {
                Status = 400,
                Title = "Validation Error",
                Detail = exception.Message
            },
            MappingException => new ProblemDetails
            {
                Status = 400,
                Title = "Mapping Error",
                Detail = exception.Message
            },
            ResourceNotFoundException => new ProblemDetails
            {
                Status = 404,
                Title = "Resource Not Found",
                Detail = exception.Message
            },
            UnauthorizedAccessException => new ProblemDetails()
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized Access",
                Detail = exception.Message
            },
            _ => new ProblemDetails()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error Occurred",
                Detail = exception?.Message
            }
        };

        await Results.Problem(
            title: problemDetails.Title,
            statusCode: problemDetails.Status,
            extensions: new Dictionary<string, object?>()
            {
                { "traceId", traceId },
                { "error", exception?.Message }
            }).ExecuteAsync(httpContext);

        return true;
    }
}