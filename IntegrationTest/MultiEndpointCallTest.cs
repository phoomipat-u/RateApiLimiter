using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using IntegrationTest.Utilities;
using Xunit;

namespace IntegrationTest
{
    public class MultiEndpointCallTest
    {
        [Fact]
        public void GivenNoPriorCallsToEndpointOneAndEndpointTwo_WhenEndpointOneReachesTheLimitForThatPeriodAndEndpointTwoIsCalled_ThenEndpointTwoCallsShouldNotBeAffectedByEndpointOneLimit()
        {
            // Arrange

            const double endpointOneLimitPeriod = 5000;
            const int endpointOneCallAmount = 25;
            const string endpointOneUri = "http://localhost:5001/hotel/city?city=Bangkok";
            const int endpointOneLimit = 10;
            
            const int endpointTwoCallAmount = 120;
            const string endpointTwoUri = "http://localhost:5001/hotel/room?room=Deluxe";
            const int endpointTwoLimit = 100;
            
            // Act
            var timer = new Stopwatch();
            timer.Start();
            
            var endpointOneResults = new List<HttpResponseMessage>();
            for (var i = 0; i < endpointOneCallAmount; i++)
            {
                endpointOneResults.Add(Helper.GenerateRequest(endpointOneUri));
            }
            
            var endpointTwoResults = new List<HttpResponseMessage>();
            for (var i = 0; i < endpointTwoCallAmount; i++)
            {
                endpointTwoResults.Add(Helper.GenerateRequest(endpointTwoUri));
            }
            
            timer.Stop();
            
            // Assert
            Debug.Assert(timer.ElapsedMilliseconds < endpointOneLimitPeriod, "Integration requests took longer than the limit period!");
            
            Assert.Equal(endpointOneLimit, endpointOneResults.Count(r => r.StatusCode == HttpStatusCode.OK));
            Assert.Equal(endpointOneCallAmount - endpointOneLimit, endpointOneResults.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests));

            Assert.Equal(endpointTwoLimit, endpointTwoResults.Count(r => r.StatusCode == HttpStatusCode.OK));
            Assert.Equal(endpointTwoCallAmount - endpointTwoLimit, endpointTwoResults.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests));
        }
    }
}