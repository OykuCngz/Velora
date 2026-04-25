using System.Net;
using System.Text.Json;
using FluentValidation;

namespace Velora.Web.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Gelen İstek: {Method} {Path}", context.Request.Method, context.Request.Path);
        
        try
        {
            await _next(context);
            _logger.LogInformation("İstek Tamamlandı: {Method} {Path} - {StatusCode}", 
                context.Request.Method, context.Request.Path, context.Response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HATA OLUŞTU: {Method} {Path} - {Message}", 
                context.Request.Method, context.Request.Path, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;

        if (exception is ValidationException validationException)
        {
            code = HttpStatusCode.BadRequest;
            result = JsonSerializer.Serialize(new { 
                errors = validationException.Errors.Select(e => new { 
                    e.PropertyName, 
                    e.ErrorMessage 
                }) 
            });
        }
        else
        {
            result = JsonSerializer.Serialize(new { error = "Sunucu tarafında beklenmedik bir hata oluştu." });
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result);
    }
}
