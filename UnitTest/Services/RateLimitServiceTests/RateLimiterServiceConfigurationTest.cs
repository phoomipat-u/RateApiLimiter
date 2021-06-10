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
    public class RateLimiterServiceConfigurationTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ILogger<RateLimiterService> _logger;
        
        public RateLimiterServiceConfigurationTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _logger = _testOutputHelper.BuildLoggerFor<RateLimiterService>();
        }

        [Fact]
        public void GivenTheDefaultConfigurationLimitIsTen_WhenTheApiCalledDoesntMatchAnyCustom_ThenTenCallsShouldReturnTrueAndAnotherFifteenCallsShouldReturnFalse()
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
            var rateLimiterService = new RateLimiterService(_logger, configurationMock.Object);

            // Act
            var results = new List<bool>();
            for (var i = 0; i < 25; i++)
            {
                var result = rateLimiterService.AllowApiCall(now, "/hotel");
                results.Add(result);
            }
            
            // Assert
            Assert.Equal(10, results.Count(r => r));
            Assert.Equal(15, results.Count(r => !r));
        }
        
        [Fact]
        public void GivenCustomLimitForHotelApiIsSetTo20_WhenTheApiPathCalledMatchesTheCustomConfiguration_ThenTwentyCallsShouldReturnTrueAndAnotherFiveCallsShouldReturnFalse()
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
                Custom = new []
                {
                    new RateLimiterConfiguration.EndpointRateLimitConfiguration()
                    {
                        Endpoint = "/hotel",
                        Limit = 20,
                        Period = 5000
                    },
                }

            });
            var rateLimiterService = new RateLimiterService(_logger, configurationMock.Object);
            
            // Act
            var results = new List<bool>();
            for (var i = 0; i < 25; i++)
            {
                var result = rateLimiterService.AllowApiCall(now, "/hotel");
                results.Add(result);
            }
            
            // Assert
            Assert.Equal(20, results.Count(r => r));
            Assert.Equal(5, results.Count(r => !r));
        }
    }
}