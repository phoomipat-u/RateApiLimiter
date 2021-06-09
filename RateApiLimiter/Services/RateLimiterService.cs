using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Timers;
using Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RateApiLimiter.Services
{
    public class RateLimiterService : IRateLimiterService
    {
        private readonly ILogger<RateLimiterService> _logger;
        private readonly RateLimiterConfiguration _rateLimiterConfiguration;
        private readonly ConcurrentDictionary<string, (DateTime, long)> _currentLimit = new();
        private readonly Dictionary<string, RateLimiterConfiguration.EndpointRateLimitConfiguration> _configurations = new();
        
        private Timer timer1;

        public RateLimiterService(ILogger<RateLimiterService> logger, IOptions<RateLimiterConfiguration> rateLimiterConfiguration)
        {
            _logger = logger;
            _rateLimiterConfiguration = rateLimiterConfiguration.Value;

        }

        public bool RateLimitApi(string path)
        {
            if (_configurations.TryGetValue(path, out var config))
            {
                var (endOfPeriodTime, counter) = _currentLimit.GetOrAdd(path, (DateTime.UtcNow.AddMilliseconds(config.Period), 0));

                if (endOfPeriodTime < DateTime.UtcNow)
                {
                    return _currentLimit.TryUpdate(path, (DateTime.UtcNow.AddMilliseconds(config.Period), 1), (endOfPeriodTime, counter)) || RateLimitApi(path);
                }
                
                if (counter >= config.Limit)
                {
                    return false;
                }

                return _currentLimit.TryUpdate(path, (endOfPeriodTime, counter+1), (endOfPeriodTime, counter)) || RateLimitApi(path);
            }
            else
            {
                _configurations[path] = _rateLimiterConfiguration.Custom.FirstOrDefault(c => c.Endpoint.ToLowerInvariant().Equals(path.ToLowerInvariant())) 
                                        ?? _rateLimiterConfiguration.Default;
                return RateLimitApi(path);
            }
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