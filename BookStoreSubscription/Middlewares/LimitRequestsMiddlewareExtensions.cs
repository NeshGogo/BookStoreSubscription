using BookStoreSubscription.DTOs;

namespace BookStoreSubscription.Middlewares
{
    public static class LimitRequestsMiddlewareExtensions
    {
        public static IApplicationBuilder UseLimitRequests(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LimitRequestsMiddleware>();
        }
    }

    public class LimitRequestsMiddleware {
        private readonly RequestDelegate next;
        private readonly IConfiguration configuration;

        public LimitRequestsMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this.next = next;
            this.configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext context)
        {
            var limitRequestsConfig = new LimitRequestConfig();
            configuration.GetRequiredSection("LimitRequests").Bind(limitRequestsConfig);
            var keyStringValues = httpContext.Request.Headers["X-API-Key"];

            if(keyStringValues.Count == 0)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("You must provide a key in the header X-API-Key");
                return;
            }
            await next(httpContext);
        }
    
    }

}
