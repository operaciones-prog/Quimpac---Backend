using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace PROYEC_QUIMPAC.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<RateLimitMiddleware> _logger;
        private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(15);
        private readonly int _maxAttempts = 5;

        public RateLimitMiddleware(RequestDelegate next, IMemoryCache cache, ILogger<RateLimitMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Solo aplicar rate limit a endpoints de login
            if (!context.Request.Path.StartsWithSegments("/api/login"))
            {
                await _next(context);
                return;
            }

            var clientIp = GetClientIpAddress(context);
            var key = $"login_attempts_{clientIp}";
            
            var attempts = _cache.Get<LoginAttempt>(key);
            
            if (attempts == null)
            {
                attempts = new LoginAttempt { Count = 0, FirstAttempt = DateTime.UtcNow };
            }

            // Si han pasado más de 15 minutos desde el primer intento, resetear contador
            if (DateTime.UtcNow - attempts.FirstAttempt > _timeWindow)
            {
                attempts = new LoginAttempt { Count = 0, FirstAttempt = DateTime.UtcNow };
            }

            // Si se excedió el límite
            if (attempts.Count >= _maxAttempts)
            {
                _logger.LogWarning("Rate limit exceeded for IP: {ClientIp}. Attempts: {Count}", 
                    clientIp, attempts.Count);

                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                await context.Response.WriteAsync("Demasiados intentos de inicio de sesión. Intente nuevamente en 15 minutos.");
                return;
            }

            // Incrementar contador antes de procesar la request
            attempts.Count++;
            _cache.Set(key, attempts, _timeWindow);

            await _next(context);

            // Si el login fue exitoso (status 200), resetear el contador
            if (context.Response.StatusCode == 200)
            {
                _cache.Remove(key);
            }
        }

        private string GetClientIpAddress(HttpContext context)
        {
            var xff = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(xff))
            {
                return xff.Split(',')[0].Trim();
            }

            var xri = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(xri))
            {
                return xri;
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }

    public class LoginAttempt
    {
        public int Count { get; set; }
        public DateTime FirstAttempt { get; set; }
    }
}