using System.Diagnostics;

namespace LogiTransPro.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var request = context.Request;

            // Obtener información del request
            var method = request.Method;
            var path = request.Path;
            var queryString = request.QueryString.ToString();
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var userAgent = request.Headers["User-Agent"].ToString();

            // Log de inicio
            _logger.LogInformation(
                "▶ Iniciando request: {Method} {Path}{Query} | IP: {IpAddress} | UserAgent: {UserAgent}",
                method, path, queryString, ipAddress, userAgent);

            try
            {
                await _next(context);
                stopwatch.Stop();

                // Log de finalización exitosa
                _logger.LogInformation(
                    "◀ Request completado: {Method} {Path} | Status: {StatusCode} | Duration: {Duration}ms",
                    method, path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex,
                    "❌ Error en request: {Method} {Path} | Duration: {Duration}ms | Error: {Error}",
                    method, path, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }
    }
}