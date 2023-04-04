using BookStoreSubscription.DTOs;
using BookStoreSubscription.Entities;
using Microsoft.EntityFrameworkCore;

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

            var route = httpContext.Request.Path.ToString();
            var existsInWhiteList = limitRequestsConfig.WhiteListRoutes.Any(x => route.Contains(x));
            if (existsInWhiteList)
            {
                await next(httpContext);
                return;
            }

            var keyStringValues = httpContext.Request.Headers["X-API-Key"];

            if (keyStringValues.Count == 0)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("You must provide a key in the header X-API-Key");
                return;
            }

            if(keyStringValues.Count > 1)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("You must provide only one key");
                return;
            }

            var key = keyStringValues[0];
            var keyDB = await context.KeyAPIs
                .Include(x => x.DomainRestrictions)
                .Include(x => x.IpRestrictions)
                .FirstOrDefaultAsync(x => x.Key == key);
            
            if (keyDB == null)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Key doesn't found");
                return;
            }

            if (!keyDB.Active)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("The key is not active");
                return;
            }

            if (keyDB.KeyType == KeyType.Free)
            {
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);
                var amountPetitionsToday = await context.Petitions.CountAsync(x => 
                    x.KeyAPIId == keyDB.Id && x.PetitionDate >= today && x.PetitionDate < tomorrow);
                if(amountPetitionsToday >= limitRequestsConfig.RequestsByDayFree) {
                    httpContext.Response.StatusCode = 429;
                    await httpContext.Response.WriteAsync("You exceeded the amount of free request per day allow. " +
                        "If you want to make more request update your subscription to a professional one.");
                    return;
                }
            }

            var requestBeatsSomeOfRestrictions = RequestBeatsSomeOfRestrictions(keyDB, httpContext);
            if (!requestBeatsSomeOfRestrictions)
            {
                httpContext.Response.StatusCode = 403;
                return;
            }

            var petition = new Petition
            {
                KeyAPIId = keyDB.Id,
                PetitionDate = DateTime.UtcNow,
            };
            await context.Petitions.AddAsync(petition);
            await context.SaveChangesAsync();
            await next(httpContext);
        }

        private bool RequestBeatsSomeOfRestrictions(KeyAPI keyAPI, HttpContext httpContext)
        {
            var thereIsRestrictions = keyAPI.DomainRestrictions.Any() || keyAPI.DomainRestrictions.Any();
            if (!thereIsRestrictions)
                return true;
            var requestBeatsDomainRestrictions = RequestBeatsDomainRestrictions(keyAPI.DomainRestrictions, httpContext);
            var requestBeatsIpRestrictions = RequestBeatsIpRestrictions(keyAPI.IpRestrictions, httpContext);
            return requestBeatsDomainRestrictions || requestBeatsIpRestrictions;
        }

        private bool RequestBeatsIpRestrictions(List<IpRestriction> restrictions, HttpContext httpContext)
        {
            if (restrictions == null || restrictions.Count == 0)
                return false;
            var ip = httpContext.Connection.RemoteIpAddress.ToString();
            if (ip == String.Empty)
                return false;

            return restrictions.Any(x => x.IP == ip);
        }

        private bool RequestBeatsDomainRestrictions(List<DomainRestriction> restrictions, HttpContext httpContext)
        {
            if(restrictions == null || restrictions.Count == 0)  
                return false; 
            var referer = httpContext.Request.Headers["Referer"].ToString();
            if(referer == String.Empty)
                return false;
           
            Uri uri = new Uri(referer);
            string host = uri.Host;

            return restrictions.Any(x => x.Domain == host);
        }

    }

}
