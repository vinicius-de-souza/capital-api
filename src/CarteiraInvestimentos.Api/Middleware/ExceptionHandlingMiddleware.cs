using CarteiraInvestimentos.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace CarteiraInvestimentos.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode statusCode;
        string message;
        LogLevel logLevel;

        switch (exception)
        {
            case NotFoundException notFoundEx:
                statusCode = HttpStatusCode.NotFound;
                message = notFoundEx.Message;
                logLevel = LogLevel.Warning;
                break;

            case ArgumentException argEx:
                statusCode = HttpStatusCode.BadRequest;
                message = argEx.Message;
                logLevel = LogLevel.Warning;
                break;

            case DomainException domainEx:
                statusCode = HttpStatusCode.BadRequest;
                message = domainEx.Message;
                logLevel = LogLevel.Warning;
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                message = "Ocorreu um erro inesperado. Tente novamente mais tarde.";
                logLevel = LogLevel.Error;
                break;
        }

        _logger.Log(logLevel, exception, "Erro durante a requisição: {Message}", message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var errorResponse = new
        {
            StatusCode = (int)statusCode,
            Message = message
        };

        var jsonResponse = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(jsonResponse);
    }
}