using Serilog.Context;

namespace CarteiraInvestimentos.Api.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[CorrelationIdHeader] = new[] { correlationId };
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}