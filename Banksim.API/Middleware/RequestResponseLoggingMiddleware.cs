using Microsoft.AspNetCore.Http;

namespace BankSim.API.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
           
            _logger.LogInformation("➡️ {Method} {Path} - IP: {IP}",
                context.Request.Method,
                context.Request.Path,
                context.Connection.RemoteIpAddress);

            await _next(context);

            
            _logger.LogInformation("⬅️ {StatusCode} {ContentType}",
                context.Response.StatusCode,
                context.Response.ContentType);
        }
    }
}
