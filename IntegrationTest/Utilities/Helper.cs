using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IntegrationTest.Utilities
{
    public class Helper
    {
        public static Task<HttpResponseMessage>[] GetConcurrentTasks(Func<Task<HttpResponseMessage>> taskFactory, int times)
        {
            var tasks = new Task<HttpResponseMessage>[times];
            for (var i = 0; i < times; i++)
            {
                tasks[i] = taskFactory.Invoke();
            }
            return tasks;
        }
        
        public static HttpResponseMessage GenerateRequest(string uri)
        {
            var client = new HttpClient();

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(uri),
                Method = HttpMethod.Get,
            };

            return client.SendAsync(request).Result;
        }
        
        public static Task<HttpResponseMessage> GenerateRequestAsync(string uri)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(uri),
                Method = HttpMethod.Get,
            };

            return client.SendAsync(request);
        }
    }
}