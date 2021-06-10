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
    public class RateLimiterServiceResetTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ILogger<RateLimiterService> _logger;
        
        public RateLimiterServiceResetTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _logger = _testOutputHelper.BuildLoggerFor<RateLimiterService>();
        }

        [Fact]
        public void GivenTheCurrentPeriodLimitExceeded_WhenCalledAgainAfterSetPeriodHasPassed_ThenTheCallCountShouldResetAndAllowAnotherSetOfCalls()
        {
            // Arrange
            var now = new DateTime(2021, 06, 10).ToUniversalTime();
            var nowFastForwardedFiveSeconds = now.AddMilliseconds(5000);
            var configurationMock = MockUtils.MockOption(new RateLimiterConfiguration()
            {
                Default = new RateLimiterConfiguration.EndpointRateLimitConfiguration()
                {
                    Limit = 10,
                    Period = 5000
                },
                Custom = Array.Empty<RateLimiterConfiguration.EndpointRateLimitConfiguration>()

            });
            var rateLimiterService = new RateLimiterService(_logger, configurationMock.Object);

            // Act
            var firstSetOfCallsResults = new List<bool>();
            for (var i = 0; i < 25; i++)
            {
                var result = rateLimiterService.AllowApiCall(now, "/hotel");
                firstSetOfCallsResults.Add(result);
            }
            
            Assert.Equal(10, firstSetOfCallsResults.Count(r => r));
            Assert.Equal(15, firstSetOfCallsResults.Count(r => !r));
            
            var secondSetOfCallsResults = new List<bool>();
            for (var i = 0; i < 25; i++)
            {
                var result = rateLimiterService.AllowApiCall(nowFastForwardedFiveSeconds, "/hotel");
                secondSetOfCallsResults.Add(result);
            }
            
            // Assert
            Assert.Equal(10, secondSetOfCallsResults.Count(r => r));
            Assert.Equal(15, secondSetOfCallsResults.Count(r => !r));
        }
    }
}