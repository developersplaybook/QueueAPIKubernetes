namespace TaskScheduler.Interfaces
{
    public interface IHttpClientFactoryService
    {
        HttpClient CreateClient(string baseUrl);
    }
}

