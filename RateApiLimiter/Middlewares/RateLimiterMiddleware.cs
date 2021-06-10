using System;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace RateApiLimiter.Middlewares
{
    public class RateLimiterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimiterMiddleware> _logger;
        private readonly IRateLimiterService _rateLimiterService;
        private int _count;

        public RateLimiterMiddleware(RequestDelegate next, ILogger<RateLimiterMiddleware> logger, IRateLimiterService rateLimiterService)
        {
            _next = next;
            _logger = logger;
            _rateLimiterService = rateLimiterService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Interlocked.Increment(ref _count);
            var path = context.Request.Path;
            
            _logger.LogTrace($"Current calls {_count} at path {path}");

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