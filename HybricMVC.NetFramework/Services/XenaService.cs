using HybricMVC.NetFramework.Configuration.Interfaces;
using HybridMVC.Core.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace HybricMVC.NetFramework.Services
{
    public class XenaService : IXenaService
    {
        private readonly IApiEndpoints _apiEndpoints;
        private readonly HttpContext _context;

        public XenaService()
        {
            _apiEndpoints = new ApiEndpoints() { Xena = ConfigurationManager.AppSettings["XenaApi"] };
            _context = HttpContext.Current;
        }

        public async Task<string> GetUserFiscalSetupAsync()
        {
            HttpClient client = SetupClientWithToken();
            var result = await client.GetStringAsync($"{_apiEndpoints.Xena}/User/FiscalSetup?forceNoPaging=true");
            return result;
        }

        public async Task<string> GetUserMembershipAsync()
        {
            HttpClient client = SetupClientWithToken();
            var result = await client.GetStringAsync($"{_apiEndpoints.Xena}/User/XenaUserMembership?ForceNoPaging=true");
            return result;
        }

        public async Task<string> GetFiscalXenaAppsAsync(string fiscalId)
        {
            HttpClient client = SetupClientWithToken();
            var result = await client.GetStringAsync($"{_apiEndpoints.Xena}/Fiscal/{fiscalId}/XenaApp");
            return result;
        }

        /// <summary>
        /// Creates HttpClient with header 'Authorization : Bearer {token}'
        /// </summary>
        /// <returns>HttpClient</returns>
        private HttpClient SetupClientWithToken()
        {
            HttpClient client = new HttpClient();
            var accessToken = ((ClaimsPrincipal)_context.User).Claims.SingleOrDefault(x => x.Type == OpenIdConnectParameterNames.AccessToken).Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return client;
        }
    }
}
