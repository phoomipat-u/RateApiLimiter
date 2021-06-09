using System;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Common.Interfaces;
using Microsoft.Extensions.Options;


namespace RateApiLimiter.Middlewares
{
    public class RateLimiterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRateLimiterService _rateLimiterService;

        public RateLimiterMiddleware(RequestDelegate next, IRateLimiterService rateLimiterService)
        {
            _next = next;
            _rateLimiterService = rateLimiterService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;

            if (_rateLimiterService.AllowApiCall(DateTime.UtcNow, path))
            {
                await _next(context);
            }
            else
            { 
                context.Response.Clear();
                context.Response.StatusCode = (int) HttpStatusCode.TooManyRequests;
                await context.Response.WriteAsync("Too Many Requests");
            }
            
        }
    }
}