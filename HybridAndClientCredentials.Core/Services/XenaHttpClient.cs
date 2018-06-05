using HybridAndClientCredentials.Core.Extensions;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace HybridAndClientCredentials.Core.Services
{
    public class XenaHttpClient
    {
        private static HttpClient _client;

        public static void Initialize(string xenaPath)
        {
            BaseAddress = xenaPath;
            _client = new HttpClient { BaseAddress = new Uri(BaseAddress) };
        }

        public static string BaseAddress { get; private set; }
        
        public async Task<string> GetStringAsync(string requestUri, string accessToken = null)
        {
            if (!string.IsNullOrWhiteSpace(accessToken)) _client.SetBearerToken(accessToken);
            return await _client.GetStringAsync(requestUri);
        }

        public async Task<byte[]> GetByteArrayAsync(string requestUri, string accessToken = null)
        {
            if (!string.IsNullOrWhiteSpace(accessToken)) _client.SetBearerToken(accessToken);
            return await _client.GetByteArrayAsync(requestUri);
        }

        public async Task<Stream> GetStreamAsync(string requestUri, string accessToken = null)
        {
            if (!string.IsNullOrWhiteSpace(accessToken)) _client.SetBearerToken(accessToken);
            return await _client.GetStreamAsync(requestUri);
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, object post, string accessToken = null)
        {
            if (!string.IsNullOrWhiteSpace(accessToken)) _client.SetBearerToken(accessToken);
            return await _client.PostAsync(requestUri, post.AsJson());
        }
    }
}