namespace PROYEC_QUIMPAC.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityHeadersMiddleware> _logger;

        public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Agregar headers de seguridad antes de procesar la request
            AddSecurityHeaders(context.Response);

            await _next(context);
        }

        private void AddSecurityHeaders(HttpResponse response)
        {
            try
            {
                // X-Content-Type-Options: Previene MIME type sniffing
                if (!response.Headers.ContainsKey("X-Content-Type-Options"))
                    response.Headers.Add("X-Content-Type-Options", "nosniff");

                // X-Frame-Options: Previene clickjacking
                if (!response.Headers.ContainsKey("X-Frame-Options"))
                    response.Headers.Add("X-Frame-Options", "DENY");

                // X-XSS-Protection: Habilita filtro XSS del browser
                if (!response.Headers.ContainsKey("X-XSS-Protection"))
                    response.Headers.Add("X-XSS-Protection", "1; mode=block");

                // Referrer-Policy: Controla información de referrer
                if (!response.Headers.ContainsKey("Referrer-Policy"))
                    response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

                // Content-Security-Policy: Política de seguridad de contenido
                if (!response.Headers.ContainsKey("Content-Security-Policy"))
                    response.Headers.Add("Content-Security-Policy", 
                        "default-src 'self'; " +
                        "script-src 'self' 'unsafe-inline'; " +
                        "style-src 'self' 'unsafe-inline'; " +
                        "font-src 'self'; " +
                        "img-src 'self' data: https:; " +
                        "connect-src 'self'; " +
                        "frame-ancestors 'none';");

                // Permissions-Policy: Controla APIs del browser
                if (!response.Headers.ContainsKey("Permissions-Policy"))
                    response.Headers.Add("Permissions-Policy", 
                        "camera=(), microphone=(), geolocation=(), payment=()");

                // Server: Ocultar información del servidor
                if (response.Headers.ContainsKey("Server"))
                    response.Headers.Remove("Server");

                // X-Powered-By: Ocultar tecnología
                if (response.Headers.ContainsKey("X-Powered-By"))
                    response.Headers.Remove("X-Powered-By");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding security headers");
            }
        }
    }
}