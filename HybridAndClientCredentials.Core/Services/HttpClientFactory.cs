using System;
using System.Net.Http;

namespace HybridAndClientCredentials.Core.Services
{
    public class HttpClientFactory : IHttpClientFactory
    {
        static string _baseAddress;

        public HttpClient CreateClient(string baseAddress)
        {
            _baseAddress = baseAddress;
            var client = new HttpClient();
            SetupClientDefaults(client);
            return client;
        }

        protected virtual void SetupClientDefaults(HttpClient client)
        {
            client.Timeout = TimeSpan.FromSeconds(30); //set your own timeout.
            client.BaseAddress = new Uri(_baseAddress);
        }
    }
}