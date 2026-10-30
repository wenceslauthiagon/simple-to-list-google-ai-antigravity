using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.Exceptions;
using TodoList.Domain.Exceptions;

namespace TodoList.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail, extensions) = exception switch
        {
            InvalidTaskIdException invalidIdEx => (
                StatusCodes.Status400BadRequest,
                "Invalid Task ID",
                invalidIdEx.Message,
                null),

            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                "Validation Error",
                validationEx.Message,
                new Dictionary<string, object?> { ["errors"] = validationEx.Errors }),

            DomainValidationException domainEx => (
                StatusCodes.Status400BadRequest,
                "Domain Validation Error",
                domainEx.Message,
                null),

            TaskNotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                "Task Not Found",
                notFoundEx.Message,
                null),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred. Please try again later.",
                null)
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning("Request failed with client error ({StatusCode}): {Message}", statusCode, exception.Message);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        if (extensions is not null)
        {
            foreach (var kvp in extensions)
            {
                problemDetails.Extensions[kvp.Key] = kvp.Value;
            }
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
