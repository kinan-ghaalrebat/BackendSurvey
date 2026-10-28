using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.Common.Exceptions;

namespace SurveyManagement.API.Middleware;

// Turns exceptions thrown anywhere in the app into clean HTTP error responses
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problem = exception switch
        {
            ValidationException ex => new ValidationProblemDetails(
                ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred."
            },
            NotFoundException ex => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found.",
                Detail = ex.Message
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred."
            }
        };

        if (problem.Status == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Unhandled exception");

        httpContext.Response.StatusCode = problem.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(problem, problem.GetType(), cancellationToken);
        return true;
    }
}
