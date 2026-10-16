using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace UfgPay.Gateway.Middlewares;

/// <summary>
/// Middleware para enriquecimento de contexto a partir dos claims JWT.
/// </summary>
public class JwtAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtAuthMiddleware> _logger;

    public JwtAuthMiddleware(RequestDelegate next, ILogger<JwtAuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var subClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                        ?? context.User.FindFirst("sub")?.Value;

            if (Guid.TryParse(subClaim, out var userId))
            {
                context.Items["UserId"] = userId;
                _logger.LogDebug("[JwtAuthMiddleware] Contexto autenticado para UserId: {UserId}", userId);
            }
        }

        await _next(context);
    }
}
