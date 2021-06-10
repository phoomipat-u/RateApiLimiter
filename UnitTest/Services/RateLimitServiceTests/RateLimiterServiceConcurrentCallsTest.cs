using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using RateApiLimiter.Services;
using TestUtility;
using Xunit;
using Xunit.Abstractions;
using ILogger = Castle.Core.Logging.ILogger;

namespace UnitTest.Services
{
    public class RateLimiterServiceConcurrentCallsTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ILogger<RateLimiterService> _logger;
        
        public RateLimiterServiceConcurrentCallsTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _logger = _testOutputHelper.BuildLoggerFor<RateLimiterService>();
        }

        [Fact]
        public async Task GivenThePathCurrentCallInTheDurationIsZeroAndTheLimitIsTenCallsPerFiveSeconds_WhenThereAreTwentyFiveConcurrentBurstCalls_ThenTenCallsShouldReturnTrueAndAnotherFifteenCallsShouldReturnFalse()
        {
            // Arrange
            var now = new DateTime(2021, 06, 10).ToUniversalTime();
            var configurationMock = MockUtils.MockOption(new RateLimiterConfiguration()
            {
                Default = new RateLimiterConfiguration.EndpointRateLimitConfiguration()
                {
                    Limit = 10,
                    Period = 5000
                },
                Custom = Array.Empty<RateLimiterConfiguration.EndpointRateLimitConfiguration>()

            });
            
            // Act
            var rateLimiterService = new RateLimiterService(_logger, configurationMock.Object);

            // Assert
            var tasks = GetConcurrentActions(() => rateLimiterService.AllowApiCall(now, "/hotel"), 25);
            var results = await Task.WhenAll(tasks);
            
            Assert.Equal(10, results.Count(r => r));
            Assert.Equal(15, results.Count(r => !r));
        }

        public Task<bool>[] GetConcurrentActions(Func<bool> action, int times)
        {
            var actionList = new List<Task<bool>>();
            for (var i = 0; i < times; i++)
            {
                actionList.Add(Task.Run(action));
            }
            return actionList.ToArray();
        }
    }
}