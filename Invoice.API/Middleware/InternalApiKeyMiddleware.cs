namespace Invoice.API.Middleware
{
    public class InternalApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _expectedApiKey;

        public InternalApiKeyMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _expectedApiKey = config["InternalApiKey"];
        }

        public async Task InvokeAsync(HttpContext context)
        {
           
            if (!context.Request.Headers.TryGetValue("X-Internal-Api-Key", out var key) || key != _expectedApiKey)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Erişim engellendi.");
                return;
            }
            await _next(context);
        }
    }

}
