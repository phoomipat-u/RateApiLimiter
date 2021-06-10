using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Timers;
using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RateApiLimiter.Services
{
    public class RateLimiterService : IRateLimiterService
    {
        private readonly ILogger<RateLimiterService> _logger;
        private readonly RateLimiterConfiguration _rateLimiterConfiguration;
        private readonly ConcurrentDictionary<string, (DateTime, long)> _currentLimit = new();
        private readonly ConcurrentDictionary<string, RateLimiterConfiguration.EndpointRateLimitConfiguration> _configurations = new();
        
        public RateLimiterService(ILogger<RateLimiterService> logger, IOptionsMonitor<RateLimiterConfiguration> rateLimiterConfiguration)
        {
            _logger = logger;
            _rateLimiterConfiguration = rateLimiterConfiguration.CurrentValue;
        }

        public bool AllowApiCall(DateTime now, string endpoint)
        {
            _logger.LogTrace($"Executed @ {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.ffffff}");
            
            var config = GetConfig(endpoint);

            do
            {
                var (endOfPeriodTime, counter) = _currentLimit.GetOrAdd(endpoint, (now.AddMilliseconds(config.Period), 0));
                if (endOfPeriodTime < now)
                {
                    if (_currentLimit.TryUpdate(endpoint, (now.AddMilliseconds(config.Period), 1), (endOfPeriodTime, counter)))
                    {
                        return true;
                    }
                }
                else if (counter >= config.Limit)
                {
                    _logger.LogTrace($"Rejected due to set limit of {config.Limit} per {config.Period}ms @ {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.ffffff}");
                    return false;
                }
                else if (_currentLimit.TryUpdate(endpoint, (endOfPeriodTime, counter + 1), (endOfPeriodTime, counter)))
                {
                    _logger.LogTrace($"Successfully Updated from {counter} to {counter+1} @ {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.ffffff}");
                    return true;
                }
                
                _logger.LogTrace($"Looping to retry {DateTime.UtcNow:MM/dd/yyyy hh:mm:ss.ffffff}");
            } while (true);
        }

        private RateLimiterConfiguration.EndpointRateLimitConfiguration GetConfig(string path)
        {
            return _configurations.GetOrAdd(path,
                p => _rateLimiterConfiguration.Custom
                            .FirstOrDefault(c => c.Endpoint.ToLowerInvariant().Equals(p.ToLowerInvariant()))
                        ?? _rateLimiterConfiguration.Default
            );
        }
    }

    public class RateLimiterConfiguration
    {
        public EndpointRateLimitConfiguration Default { get; set; }
        public EndpointRateLimitConfiguration[] Custom { get; set; }
        
        
        public class EndpointRateLimitConfiguration
        {
            public string Endpoint { get; set; }
            public double Period { get; set; }
            public int Limit { get; set; }
        }
    }
}