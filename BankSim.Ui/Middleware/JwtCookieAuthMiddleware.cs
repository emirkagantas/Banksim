namespace BankSim.Ui.Middleware
{
    public class JwtCookieAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtCookieAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Cookies["token"];
            if (!string.IsNullOrEmpty(token))
            {
        
                if (!context.Request.Headers.ContainsKey("Authorization"))
                {
                    context.Request.Headers.Append("Authorization", "Bearer " + token);
                }
            }

            await _next(context);
        }
    }
}
