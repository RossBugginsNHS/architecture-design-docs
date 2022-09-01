using Microsoft.AspNetCore.Http;
namespace dhc;
public class ForwardedPrefixBasePathMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ForwardedPrefixBasePathMiddleware> _logger;
    public ForwardedPrefixBasePathMiddleware(RequestDelegate next, ILogger<ForwardedPrefixBasePathMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-Prefix", out var pathBase))
        {
            _logger.LogInformation("Got X-Forwarded-Prefix of {prefix}", pathBase);
            context.Request.PathBase = pathBase.Last();

            if (context.Request.Path.StartsWithSegments(context.Request.PathBase, out var path))
            {
                context.Request.Path = path;
            }
        }
        else
        {
            _logger.LogInformation("Not got X-Forwarded-Prefix of {prefix}", pathBase);
        }

        await _next(context);
    }
}
