using System.Net;
using System.Text.Json;
using BusinessUnits.Domain.Exceptions;
using FluentValidation;

namespace BusinessUnits.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        => (_next, _logger) = (next, logger);

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (status, message, errors) = exception switch
        {
            ValidationException ve => (HttpStatusCode.UnprocessableEntity,
                "Validation failed",
                ve.Errors.Select(e => e.ErrorMessage).ToArray()),
            NotFoundException nfe => (HttpStatusCode.NotFound, nfe.Message, Array.Empty<string>()),
            DomainException de => (HttpStatusCode.BadRequest, de.Message, Array.Empty<string>()),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.", Array.Empty<string>())
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var response = new
        {
            status = (int)status,
            message,
            errors
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
