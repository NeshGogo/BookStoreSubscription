﻿using BookStoreSubscription.DTOs;
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
            var keyDB = await context.KeyAPIs.FirstOrDefaultAsync(x => x.Key == key);
            
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

            var petition = new Petition
            {
                KeyAPIId = keyDB.Id,
                PetitionDate = DateTime.UtcNow,
            };
            await context.Petitions.AddAsync(petition);
            await context.SaveChangesAsync();
            await next(httpContext);
        }
    
    }

}
