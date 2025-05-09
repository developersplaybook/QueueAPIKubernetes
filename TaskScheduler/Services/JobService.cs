using TaskScheduler.Interfaces;
using TaskScheduler.Models;

namespace TaskScheduler.Services
{
    public class JobService : IJobService
    {
        private readonly IHttpClientFactoryService _httpClientFactory;
        private static readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(10000);

        public JobService(IHttpClientFactoryService httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task TriggerJobToggle(JobNames jobName)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ClientApi");
                client.Timeout = Timeout;

                var uriBuilder = new UriBuilder($"{client.BaseAddress}jobstatus/toggle")
                {
                    Query = $"jobName={Uri.EscapeDataString(jobName.ToString())}"
                };

                var response = await client.GetAsync(uriBuilder.Uri);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Trigger 'TriggerJobToggle' was not successful. {response.StatusCode} {content}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
