using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using IntegrationTest.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTest
{
    public class SequentialCallTest
    {
        public SequentialCallTest(ITestOutputHelper testOutputHelper) { }

        [Fact]
        public void GivenNoPriorCallsToTheEndpointAndLimitIsTenCallsPerFiveSeconds_WhenCallingTwentyFiveSequentialBurstRequests_ThenShouldReturnTenGoodResponseAndFifteenDenialResponses()
        {
            const double endpointLimitPeriod = 5000;
            
            var timer = new Stopwatch();
            timer.Start();
            
            var results = new List<HttpResponseMessage>();
            for (var i = 0; i < 25; i++)
            {
                results.Add(Helper.GenerateRequest("http://localhost:5001/hotel/city?city=Bangkok"));
            }
            timer.Stop();

            Debug.Assert(timer.ElapsedMilliseconds < endpointLimitPeriod,  "Integration requests took longer than the limit period!");
            Assert.Equal(10, results.Count(r => r.StatusCode == HttpStatusCode.OK));
            Assert.Equal(15, results.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests));
        }
    }
}