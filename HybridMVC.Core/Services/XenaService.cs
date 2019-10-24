using HybridMVC.Core.Configuration;
using HybridMVC.Core.Configuration.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http;
using System.Threading.Tasks;

namespace HybridMVC.Core.Services
{
    public class XenaService : IXenaService
    {
        private readonly IApiEndpoints _apiEndpoints;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpContext _context;

        public XenaService(IOptions<ApiEndpoints> apiEndpointsOptions, IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor)
        {
            _apiEndpoints = apiEndpointsOptions.Value;
            _httpClientFactory = httpClientFactory;
            _context = contextAccessor.HttpContext;
        }

        public async Task<string> GetUserFiscalSetupAsync()
        {
            HttpClient client = await SetupClientWithToken();
            var result = await client.GetStringAsync($"{_apiEndpoints.Xena}/User/FiscalSetup?forceNoPaging=true");
            return result;
        }

        public async Task<string> GetUserMembershipAsync()
        {
            HttpClient client = await SetupClientWithToken();
            var result = await client.GetStringAsync($"{_apiEndpoints.Xena}/User/XenaUserMembership?ForceNoPaging=true");
            return result;
        }

        public async Task<string> GetFiscalXenaAppsAsync(string fiscalId) 
        {
            HttpClient client = await SetupClientWithToken();
            var result = await client.GetStringAsync($"{_apiEndpoints.Xena}/Fiscal/{fiscalId}/XenaApp");
            return result;
        }

        /// <summary>
        /// Creates HttpClient with header 'Authorization : Bearer {token}'
        /// </summary>
        /// <returns>HttpClient</returns>
        private async Task<HttpClient> SetupClientWithToken()
        {
            var client = _httpClientFactory.CreateClient();
            var accessToken = await _context.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            client.SetBearerToken(accessToken);
            return client;
        }
    }
}
