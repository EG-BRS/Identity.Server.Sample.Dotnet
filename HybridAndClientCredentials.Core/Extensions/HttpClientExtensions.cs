using HybridAndClientCredentials.Core.Models;
using System.Net.Http;
using System.Net.Http.Headers;

namespace HybridAndClientCredentials.Core.Extensions
{
    public static class HttpClientExtensions
    {
        public static void SetBearerToken(this HttpClient client, string accessToken)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(IdentityServerConstants.AuthenticationHeader.Bearer, accessToken);
        }
    }
}
