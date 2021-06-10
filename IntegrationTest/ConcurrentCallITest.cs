using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IntegrationTest.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTest
{
    public class ConcurrentCallTest
    {
        public ConcurrentCallTest(ITestOutputHelper testOutputHelper) { }
        
        [Fact]
        public async Task GivenNoPriorCallsToTheEndpointAndLimitIsTenCallsPerFiveSeconds_WhenCallingTwentyFiveConcurrentBurstHttpRequests_ShouldReturnTenGoodResponseAndFifteenDenialResponses()
        {
            const double endpointLimitPeriod = 5000;
            
            var timer = new Stopwatch();
            timer.Start();
            
            var tasks = Helper.GetConcurrentTasks(() => Helper.GenerateRequestAsync("http://localhost:5001/hotel/city?city=Bangkok"), 25);
            var results = await Task.WhenAll(tasks);

            timer.Stop();
            Debug.Assert(timer.ElapsedMilliseconds < endpointLimitPeriod, "Integration requests took longer than the limit period!");
            
            Assert.Equal(10, results.Count(r => r.StatusCode == HttpStatusCode.OK ));
            Assert.Equal(15, results.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests));
        }
    }
}