using System.Net;
using System.Text.Json;
using Application.Response;
using Domain.Enum;
using Domain.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", context.TraceIdentifier);

        var (statusCode, title, detail) = exception switch
        {
            ValidationException => ((int)HttpStatusCode.BadRequest, "Validation error", exception.Message),
            _ => ((int)HttpStatusCode.InternalServerError, "Internal server error", BusinessError.InternalServerError.GetDescription())
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7807",
            Title = title,
            Detail = detail,
            Status = statusCode,
            Instance = context.Request.Path,
            Extensions = { ["traceId"] = context.TraceIdentifier }
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, JsonOptions));
    }
}
