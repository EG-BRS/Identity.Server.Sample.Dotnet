using System.Net.Http;

namespace HybridAndClientCredentials.Core.Services
{
    public interface IHttpClientFactory
    {
        HttpClient CreateClient(string baseAddress);
    }
}