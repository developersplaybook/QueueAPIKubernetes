using TaskScheduler.Interfaces;

namespace TaskScheduler.Models
{
    public class HttpClientFactoryService : IHttpClientFactoryService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientFactoryService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public HttpClient CreateClient(string clientId)
        {
            var client = _httpClientFactory.CreateClient(clientId);
            return client;
        }
    }
}
