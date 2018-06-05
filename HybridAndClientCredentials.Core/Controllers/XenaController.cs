using System;
using System.Net.Http;
using System.Threading.Tasks;
using HybridAndClientCredentials.Core.Extensions;
using HybridAndClientCredentials.Core.Models;
using HybridAndClientCredentials.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HybridAndClientCredentials.Core.Controllers
{
    public class XenaController : Controller
    {
        private readonly XenaHttpClient _client;
        private readonly Credentials _credentials;
        private readonly IConfiguration _configuration;
        

        public XenaController(XenaHttpClient client, Credentials credentials, IConfiguration configuration)
        {
            _client = client;
            _credentials = credentials;
            _configuration = configuration;
            
        }

        public IActionResult Error()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> CallXena()
        {
            var accessToken = await HttpContext.GetTokenAsync(IdentityServerConstants.HttpContextHeaders.AccessToken);
            var client = new HttpClient();
            client.SetBearerToken(accessToken);
            var content = await client.GetStringAsync($"{_configuration["ServiceSettings:XenaAPIEndpoint"]}/Api/User/FiscalSetup?forceNoPaging=true");
            ViewBag.Json = content;
            return View("json");
        }

        [Authorize]
        public async Task<IActionResult> XenaMembership()
        {
            var accessToken = await HttpContext.GetTokenAsync(IdentityServerConstants.HttpContextHeaders.AccessToken);
            var client = new HttpClient();

            var x = _configuration["ServiceSettings:XenaAPIEndpoint"];

            client.SetBearerToken(accessToken);
            try
            {
                var content = await client.GetStringAsync($"{_configuration["ServiceSettings:XenaAPIEndpoint"]}/Api/User/XenaUserMembership?ForceNoPaging=true");
                ViewBag.Json = content;
                return View("json");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [Authorize]
        public async Task<IActionResult> GetApps()
        {
            var fiscalId = HttpContext.User?.FindFirst("xena_fiscal_id")?.Value;

            var accessToken = await HttpContext.GetTokenAsync(IdentityServerConstants.HttpContextHeaders.AccessToken);
            var client = new HttpClient();
            client.SetBearerToken(accessToken);
            var content = await client.GetStringAsync($"{_configuration["ServiceSettings:XenaAPIEndpoint"]}/Api/Fiscal/{fiscalId}/XenaApp");
            ViewBag.Json = content;
            return View("json");
        }
    }
}
