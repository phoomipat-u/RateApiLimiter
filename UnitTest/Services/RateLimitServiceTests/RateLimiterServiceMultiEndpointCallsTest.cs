using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using RateApiLimiter.Services;
using TestUtility;
using Xunit;
using Xunit.Abstractions;

namespace UnitTest.Services
{
    public class MultiEndpointCallsTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ILogger<RateLimiterService> _logger;
        
        public MultiEndpointCallsTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _logger = _testOutputHelper.BuildLoggerFor<RateLimiterService>();
        }

        [Fact]
        public void GivenThePathCurrentCallInTheDurationIsZeroAndTheLimitIsTenCallsPerFiveSeconds_WhenThereAreTwentyFiveContinuouslySequentialCalls_ThenTenCallsShouldReturnTrueAndAnotherFifteenCallsShouldReturnFalse()
        {
            // Arrange
            const int callsPerEndpoint = 25;

            const string endpointOnePath = "endpointOne";
            const int endpointOneLimit = 20;
            
            const string endpointTwoPath = "endpointTwo";
            const int endpointTwoLimit = 10;
            
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
                        Endpoint = endpointOnePath,
                        Limit = endpointOneLimit,
                        Period = 5000
                    },
                    new RateLimiterConfiguration.EndpointRateLimitConfiguration()
                    {
                        Endpoint = endpointTwoPath,
                        Limit = endpointTwoLimit,
                        Period = 5000
                    },
                }

            });
            var rateLimiterService = new RateLimiterService(_logger, configurationMock.Object);

            // Act
            var resultOfEndpointOne = new List<bool>();
            for (var i = 0; i < 25; i++)
            {
                var result = rateLimiterService.AllowApiCall(now, endpointOnePath);
                resultOfEndpointOne.Add(result);
            }
            
            var resultOfEndpointTwo = new List<bool>();
            for (var i = 0; i < 25; i++)
            {
                var result = rateLimiterService.AllowApiCall(now, endpointTwoPath);
                resultOfEndpointTwo.Add(result);
            }
            
            // Assert
            Assert.Equal(endpointOneLimit, resultOfEndpointOne.Count(r => r));
            Assert.Equal(callsPerEndpoint - endpointOneLimit, resultOfEndpointOne.Count(r => !r));
            
            Assert.Equal(endpointTwoLimit, resultOfEndpointTwo.Count(r => r));
            Assert.Equal(callsPerEndpoint - endpointTwoLimit, resultOfEndpointTwo.Count(r => !r));
        }
    }
}